using UnityEngine;
using System.Collections.Generic;

public class NeuralNetwork
{
    int inputsNumber;
    int outputsNumber;
    List<float> nnInputs;
    List<float> nnOutputs;
    List<NeuralLayer>  hiddenLayers;
    NeuralLayer inputLayer;
    NeuralLayer outputLayer;

    public NeuralNetwork()
    {
        inputsNumber = 0;
        outputsNumber = 0;
        inputLayer = new NeuralLayer();
        outputLayer = new NeuralLayer();
        nnInputs = new List<float>();
        nnOutputs = new List<float>();
        hiddenLayers = new List<NeuralLayer>();
    }

    public List<float> Inputs
    {
        get { return nnInputs; }
        set { nnInputs = value; }
    }

    public List<float> Outputs
    {
        get { return nnOutputs; }
    }

    public int HiddenLayersNumber
    {
        get { return hiddenLayers.Count; }
    }

    public int OutputsNumber
    {
        get { return outputsNumber; }
    }

    public void Create(int[] networkStructure)
    {
        inputsNumber = networkStructure[0];
        outputsNumber = networkStructure[networkStructure.Length-1];
        int hiddenLayers = networkStructure.Length - 2;
        for (int i = 0; i < hiddenLayers; i++)
        {
            NeuralLayer layer = new NeuralLayer();
            layer.PopulateLayer(networkStructure[i+1], networkStructure[i]);
            this.hiddenLayers.Add(layer);
        }        
        outputLayer.PopulateLayer(networkStructure[networkStructure.Length-1], networkStructure[networkStructure.Length-2]);
    }

    public void Calculate()
    {
        nnOutputs.Clear();
        for (int i = 0; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i].Calculate(nnInputs, ref nnOutputs);
            nnInputs = nnOutputs;
        }
        outputLayer.Calculate(nnInputs, ref nnOutputs);
    }

    public void SetFromGenomes(Genome genome, int[] networkStructure)
    {
        ClearNN();
        inputsNumber = networkStructure[0];
        outputsNumber = networkStructure[networkStructure.Length - 1];
        int index = 0;
        int hiddenLayers = networkStructure.Length - 2;
        for (int i = 0; i < hiddenLayers; i++)
        {
            NeuralLayer hidden = new NeuralLayer();
            List<Neuron> neurons = new List<Neuron>();
            int hiddenNeurons = networkStructure[i + 1];
            for (int j = 0; j < hiddenNeurons; j++)
            {
                neurons.Add(new Neuron());
                List<float> weights = new List<float>();
                for (int k = 0; k < networkStructure[i] + 1; k++)
                {
                    weights.Add(genome.Weights[index]);
                    index++;
                }
                neurons[j].Initialize(weights, networkStructure[i]);
            }
            hidden.LoadLayer(neurons);
            this.hiddenLayers.Add(hidden);
        }
        SetOutputLayer(ref genome, networkStructure[networkStructure.Length - 1], networkStructure[networkStructure.Length - 2], index);
    }

    void SetOutputLayer(ref Genome genome, int numberOutputs, int numberInputs, int index)
    {
        List<Neuron> outneurons = new List<Neuron>();
        for (int i = 0; i < numberOutputs; i++)
        {
            outneurons.Add(new Neuron());
            List<float> weights = new List<float>();
            for (int j = 0; j < numberInputs + 1; j++)
            {
                weights.Add(genome.Weights[index]);
                index++;
            }
            outneurons[i].Initialize(weights, numberInputs);
        }
        outputLayer = new NeuralLayer();
        outputLayer.LoadLayer(outneurons);
    }

    void ClearNN()
    {
        inputLayer = new NeuralLayer();
        outputLayer = new NeuralLayer();
        hiddenLayers = new List<NeuralLayer>();
    }    
}
