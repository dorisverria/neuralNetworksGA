using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class GeneticAlgorithm
{
    List<Genome> populationGenomes;
    Genome hero;
    int[]  networkStructure;
    int generationIndex;           
    int genomeIndex;
    int timesHeroSpawned;
    int totalWeightsNumber;

    public GeneticAlgorithm(int[] networkStructure)
    {
        genomeIndex = 0;
        generationIndex = 1;
        this.networkStructure = networkStructure;
        SetTotalWeights();
        populationGenomes = new List<Genome>();
    }

    public int TotalWeights
    {
        get { return totalWeightsNumber; }
    }

    public int Generation
    {
        get { return generationIndex; }
    }

    public int TimesHeroUsed
    {
        get { return timesHeroSpawned; }
    }

    public List<Genome> Population
    {
        get { return populationGenomes; }
    }

    static public float RandomFloat()
    {
        float rand = (float)Random.Range(0.0f, 32767.0f);
        return rand / 32767.0f + 1.0f;
    }

    static public float RandomClamped()
    {
        return RandomFloat() - RandomFloat();
    }

    public Genome CreateGenomeFromData(FileIO.GenomeData data)
    {
        if (data.weights.Count != 0)
        {
            Genome bestGenome = CreateGenomeWithoutWeights();
            bestGenome.Weights = data.weights;
            return bestGenome;
        }
        else
        {
            return CreateNewGenome();
        }
    }

    public void LoadGenome(ref FileIO.GenomeData data)
    {
        if (data.weights.Count != 0)
        {
            Genome bestGenome = CreateGenomeWithoutWeights();
            bestGenome.Weights = data.weights;
            if (populationGenomes.Count > 0)
            {
                int worstGenome = 0;
                float fitness = populationGenomes[0].Fitness;
                for (int i = 1; i < populationGenomes.Count; i++)
                {
                    if (populationGenomes[i].Fitness < fitness)
                    {
                        fitness = populationGenomes[i].Fitness;
                        worstGenome = i;
                    }
                }
                populationGenomes[worstGenome].Index = bestGenome.Index;
                populationGenomes[worstGenome].Weights = bestGenome.Weights;
            }
            else
            {
                populationGenomes.Add(bestGenome);
            }
        }
    }

    public Genome GetBestGenome()
    {
        int bestGenome = 0;
        float fitness = populationGenomes[0].Fitness;
        for (int i = 1; i < populationGenomes.Count; i++)
        {
            if (populationGenomes[i].Fitness > fitness)
            {
                fitness = populationGenomes[i].Fitness;
                bestGenome = i;
            }
        }
        return populationGenomes[bestGenome];
    }

    public void SetGenomeFitness(float fitness, int index)
    {
        populationGenomes[index].Fitness = fitness;
    }

    Genome CreateGenomeWithoutWeights()
    {
        Genome child = new Genome();
        child.Index = genomeIndex;
        child.Weights = new List<float>(totalWeightsNumber);
        for (int i = 0; i < totalWeightsNumber; i++)
        {
            child.Weights.Add(0.0f);
        }
        genomeIndex++;
        return child;
    }

    Genome CreateNewGenome()
    {
        Genome genome = new Genome();
        genome.Index = genomeIndex;
        genome.Fitness = 0.0f;
        genome.Weights = new List<float>(totalWeightsNumber);
        for (int i = 0; i < totalWeightsNumber; i++)
        {
            genome.Weights.Add(RandomClamped());
        }
        genomeIndex++;
        return genome;
    }

    void SetTotalWeights()
    {
        totalWeightsNumber = 0;
        for (int i = 0; i < networkStructure.Length - 1; i++)
        {
            int j = i + 1;
            totalWeightsNumber += networkStructure[i] * networkStructure[j] + networkStructure[j];
        }
    }

    void CrossOver(Genome genome1, Genome genome2, ref Genome child1, ref Genome child2)
    {
        int crossover = (int)Random.Range(0, totalWeightsNumber - 1);
        child1 = CreateGenomeWithoutWeights();
        child2 = CreateGenomeWithoutWeights();
        for (int i = 0; i < crossover; i++)
        {
            child1.Weights[i] = genome1.Weights[i];
            child2.Weights[i] = genome2.Weights[i];
        }
        for (int i = crossover; i < totalWeightsNumber; i++)
        {
            child1.Weights[i] = genome2.Weights[i];
            child2.Weights[i] = genome1.Weights[i];
        }
    }

    void Mutate(Genome genome)
    {
        for (int i = 0; i < genome.Weights.Count; i++)
        {
            if (Random.Range(0.00000f, 1.00001f) < 0.1f)
            {
                genome.Weights[i] += RandomClamped() * 0.3f;
            }
        }
    }

    void BreedParents(ref List<Genome> bestGenomes, ref List<Genome> children, ref bool containsHero)
    {
        Genome child1 = new Genome();
        Genome child2 = new Genome();
        for (int i = 0; i < bestGenomes.Count; i++)
        {
            if (Random.Range(0.0f, 1.1f) > 1.0f)
            {
                children.Add(bestGenomes[i]);
                if (children.Last().Index == hero.Index)
                {
                    containsHero = true;
                }
            }
            else
            {
                if (i < 2)
                {
                    children.Add(bestGenomes[i]);
                    if (children.Last().Index == hero.Index)
                    {
                        containsHero = true;
                    }
                }
                for (int j = i + 1; j < bestGenomes.Count; j++)
                {
                    CrossOver(bestGenomes[i], bestGenomes[j], ref child1, ref child2);
                    Mutate(child1);
                    Mutate(child2);
                    children.Add(child1);
                    children.Add(child2);
                }
            }
        }
    }

    public void GenerateNewPopulation(int totalPopulation)
    {
        ClearPopulation();
        populationGenomes.Capacity = totalPopulation;
        genomeIndex = 0;
        generationIndex = 1;
        int count = totalPopulation - populationGenomes.Count;
        for (int i = 0; i < count; i++)
        {
            populationGenomes.Add(CreateNewGenome());
        }
        hero = new Genome();
    }      

    public void BreedPopulation()
    {
        List<Genome> bestGenomes = new List<Genome>(3);
        ElitismSelection(3, ref bestGenomes);
        if (bestGenomes.Count != 0)
        {
            SelectHero(ref bestGenomes);
            bool containsHero = false;
            List<Genome> children = new List<Genome>(populationGenomes.Count);
            BreedParents(ref bestGenomes, ref children, ref containsHero);
            if (!containsHero)
            {
                SetupHero(ref children);
            }
            int remainingChildren = (populationGenomes.Count - children.Count);
            for (int i = 0; i < remainingChildren; i++)
            {
                children.Add(CreateNewGenome());
            }
            ClearPopulation();
            populationGenomes = children;
        }
        generationIndex++;
    }

    void ElitismSelection(int totalGenomes, ref List<Genome> output)
    {
        int sizeLeft = totalGenomes - output.Count;
        for (int i = 0; i < sizeLeft; i++)
        {
            float bestFitness = 0;
            int bestIndex = -1;
            for (int j = 0; j < populationGenomes.Count; j++)
            {
                if (Population[j].Fitness >= bestFitness)
                {
                    bool used = false;
                    for (int k = 0; k < output.Count; k++)
                    {
                        if (output[k].Index == Population[j].Index)
                        {
                            used = true;
                        }
                    }
                    if (!used)
                    {
                        bestIndex = j;
                        bestFitness = Population[bestIndex].Fitness;
                    }
                }
            }
            if (bestFitness == 0)
            {
                break;
            }
            if (bestIndex != -1)
            {
                output.Add(Population[bestIndex]);
            }
        }
    }

    public void ClearPopulation()
    {
        populationGenomes.Clear();
    }    

    void SelectHero(ref List<Genome> bestGenomes)
    {
        if (bestGenomes[0].Fitness > hero.Fitness)
        {
            hero.Index = bestGenomes[0].Index;
            hero.Fitness = bestGenomes[0].Fitness;
            hero.Weights = bestGenomes[0].Weights;
        }
    }    

    void SetupHero(ref List<Genome> children)
    {
        if (children[0].Fitness < hero.Fitness * 0.65f)
        {
            Genome hero = new Genome();
            hero.Index = this.hero.Index;
            hero.Weights = this.hero.Weights;
            children.Add(hero);
            timesHeroSpawned++;
        }
    }
    
    
}
