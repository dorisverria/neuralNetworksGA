using System.Collections.Generic;


public class Genome
{
    float genomeFitness;
    int genomeIndex;
    List<float> genomeWeights;

    public Genome()
    {
        genomeFitness = 0.0f;
        genomeIndex = 0;
        genomeWeights = new List<float>();
    }

    public float Fitness
    {
        get { return genomeFitness; }
        set { genomeFitness = value; }
    }

    public int Index
    {
        get { return genomeIndex; }
        set { genomeIndex = value; }
    }

    public List<float> Weights
    {
        get { return genomeWeights; }
        set { genomeWeights = value; }
    }
}
