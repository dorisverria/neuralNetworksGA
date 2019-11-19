using UnityEngine;
using System.Collections.Generic;


public class Neuron 
{
    int inputs;
    List<float>  neuronWeights;   

    public Neuron()
    {
        inputs = 0;
        neuronWeights = new List<float>();
    }

    public int InputNumber
    {
        get { return inputs; }
    }

    public List<float> Weights
    {
        get { return neuronWeights; }
    }

    public void Initialize(List<float> inputWeights, int inputsNumber)
    {
        inputs = inputsNumber;
        neuronWeights = inputWeights;
    }

    public void Populate(int inputsNumber)
    {
        inputs = inputsNumber;
        for (int i = 0; i < inputsNumber; i++)
        {
            neuronWeights.Add(GeneticAlgorithm.RandomClamped());
        }
        neuronWeights.Add(GeneticAlgorithm.RandomClamped());
    }
}
