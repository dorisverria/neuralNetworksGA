using UnityEngine;
using System.Collections.Generic;

public class EvolutionController : MonoBehaviour
{
    List<FileIO.ImprovementData> dataTracked;
    List<NeuralNetworkPlayer> nnAgents;
    GeneticAlgorithm geneticAlgorithm;
    int[] neuralNet;
    float currentBestFitness;
    float bestFitness;
    float timeUntilEvolution;
    int genomesAlive;
    int generationsSinceImprovement;
    int tests;
    bool displayText;
    bool newPopulation;
    bool saveBestGenome;
    [SerializeField]
    float[] _fitnesses;
    [SerializeField]
    UnityEngine.UI.Text _textGA_data;
    [SerializeField]
    int _generationToLoad;
   
    public void Initialize(List<NeuralNetworkPlayer> agents)
    {
        _fitnesses = new float[agents.Count];
        neuralNet = new int[] { 7, 2 };
        nnAgents = agents;
        InitializeVariables();

        if (GameManager.gameMode == GameManager.GameMode.NeuralNetworkTraining)
        {
            geneticAlgorithm.GenerateNewPopulation(nnAgents.Count);
            InsertBestGenomeInPopulation();
            SetPopulation();
        }
        else
        {
            SetupBestGenomes();
        }
        _textGA_data.text = "";
    }

    public void ManualUpdate()
    {
        UpdateAgents();
        CheckForBestFitness();
        EvolutionTimer();
        UpdateText();
    }

    void InitializeVariables()
    {
        displayText = true;
        newPopulation = false;
        saveBestGenome = false;
        dataTracked = new List<FileIO.ImprovementData>();
        geneticAlgorithm = new GeneticAlgorithm(neuralNet);
        currentBestFitness = 0.0f;
        bestFitness = currentBestFitness;
        genomesAlive = nnAgents.Count;
        generationsSinceImprovement = 0;
        timeUntilEvolution = 180.0f;
        tests = 0;
    }

    void InsertBestGenomeInPopulation()
    {
        FileIO.GenomeData data = LoadBestGenome();
        if (data != null)
        {
            geneticAlgorithm.LoadGenome(ref data);
        }
    }

    void SetPopulation()
    {
        List<Genome> population = geneticAlgorithm.Population;
        for (int i = 0; i < nnAgents.Count; i++)
        {
            NeuralNetwork neuralNetwork = new NeuralNetwork();
            neuralNetwork.SetFromGenomes(population[i], neuralNet);
            nnAgents[i].InitializeNeuralNetwork(neuralNetwork);
        }
    }

    void SetupBestGenomes()
    {
        FileIO.GenomeData data = LoadBestGenome();
        NeuralNetwork neuralNetwork = new NeuralNetwork();
        if (data != null)
        {
            Genome genome = geneticAlgorithm.CreateGenomeFromData(LoadBestGenome());
            neuralNetwork.SetFromGenomes(genome, neuralNet);
            for (int i = 0; i < nnAgents.Count; i++)
            {
                nnAgents[i].InitializeNeuralNetwork(neuralNetwork);
            }
        }
        else
        {
            for (int i = 0; i < nnAgents.Count; i++)
            {
                neuralNetwork.Create(neuralNet);
                nnAgents[i].InitializeNeuralNetwork(neuralNetwork);
            }
        }
    }

    void UpdateAgents()
    {
        currentBestFitness = 0;
        genomesAlive = 0;

        for (int i = 0; i < nnAgents.Count; i++)
        {
            if (nnAgents[i].alive)
            {
                genomesAlive++;
            }

            _fitnesses[i] = nnAgents[i].fitness;
            if (nnAgents[i].fitness > currentBestFitness)
            {
                currentBestFitness = nnAgents[i].fitness;
            }
        } 
    }

    void CheckForBestFitness()
    {
        if (currentBestFitness > bestFitness)
        {
            bestFitness = currentBestFitness;

            if (generationsSinceImprovement > 1)
            {
                dataTracked.Add(new FileIO.ImprovementData(currentBestFitness, geneticAlgorithm.Generation, generationsSinceImprovement));
                saveBestGenome = true;
            }

            generationsSinceImprovement = 0;
        }
    }

    void EvolutionTimer()
    {
        if (genomesAlive == 0 || timeUntilEvolution < 0)
        {
            if (timeUntilEvolution < 0 || saveBestGenome)
            {
                if (timeUntilEvolution < 0)
                {
                    SaveEvolutionData();
                    tests++;
                }
                SaveBestGenome();
                saveBestGenome = false;
            }
            Evolve();
        }
        else
        {
            timeUntilEvolution -= Time.deltaTime;
        }
    }

    void UpdateText()
    {
        if (displayText)
        {
            _textGA_data.text = "CurrentBestFitness: " + currentBestFitness + "\n" +
                         "BestFitness: " + bestFitness + "\n" +
                         "Generation: " + geneticAlgorithm.Generation + "\n" +
                         "Generations since Last Improvement: " + generationsSinceImprovement + "\n";
        }
        else
        {
            _textGA_data.text = "";
        }
    }

    void Evolve()
    {
        newPopulation = true;
        Breed();
        SetPopulation();
        currentBestFitness = 0;
        timeUntilEvolution = 180.0f;
        generationsSinceImprovement++;
    }

    void Breed()
    {
        for (int i = 0; i < nnAgents.Count; i++)
        {
            geneticAlgorithm.SetGenomeFitness(nnAgents[i].fitness, i);
        }
        geneticAlgorithm.BreedPopulation();
    }

    FileIO.GenomeData LoadBestGenome()
    {
        string path = "BestGenomeWeights-" + geneticAlgorithm.TotalWeights + "_Generation-" + _generationToLoad;
        return FileIO.ReadJson<FileIO.GenomeData>(path + ".json");
    }

    void SaveBestGenome()
    {
        Genome bestGenome = geneticAlgorithm.GetBestGenome();
        FileIO.GenomeData data = new FileIO.GenomeData(bestGenome.Weights);
        string path = "BestGenomeWeights-" + data.weights.Count + "_Generation-" + geneticAlgorithm.Generation;
        FileIO.WriteJson(path + ".json", ref data);
    }

    void SaveEvolutionData()
    {
        FileIO.EvolutionData data = new FileIO.EvolutionData(dataTracked, genomesAlive, geneticAlgorithm.TimesHeroUsed);
        string path = "EvolutionData" + tests;
        FileIO.WriteJson(path + ".json", ref data);
    }

    public bool newGeneration
    {
        get { return newPopulation; }
        set { newPopulation = value; }
    }
}
