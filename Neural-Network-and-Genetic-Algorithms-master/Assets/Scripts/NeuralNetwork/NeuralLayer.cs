using UnityEngine;
using System.Collections.Generic;


public class NeuralLayer
{
    int totalNeurons;
    int totalInputs;
    List<Neuron> layerNeurons;

    public NeuralLayer()
    {
        totalNeurons = 0;
        totalInputs = 0;
        layerNeurons = new List<Neuron>();
    }

    public void Calculate(List<float> input, ref List<float> output)
    {
        int inputIndex = 0;
        List<float> outputs = new List<float>(totalNeurons);
        for (int i = 0; i < totalNeurons; i++)
        {
            float activation = 0.0f;
            for (int j = 0; j < layerNeurons[i].InputNumber - 1; j++)
            {
                activation += input[inputIndex] * layerNeurons[i].Weights[j];
                inputIndex++;
            }
            activation += layerNeurons[i].Weights[layerNeurons[i].InputNumber] * (-1.0f);
            outputs.Add(Sigmoid(activation, 1.0f));
            inputIndex = 0;
        }
        output = outputs;
    }

    public float Sigmoid(float x, float y)
    {
        float xy = (-x) / y;
        return (1 / (1 + Mathf.Exp(xy)));
    }

    public void PopulateLayer(int numberNeurons, int numberInputs)
    {
        totalInputs = numberInputs;
        totalNeurons = numberNeurons;
        if (layerNeurons.Count < numberNeurons)
        {
            for (int i = 0; i < numberNeurons; i++)
            {
                layerNeurons.Add(new Neuron());
            }
        }
        for (int i = 0; i < numberNeurons; i++)
        {
            layerNeurons[i].Populate(numberInputs);
        }
    }

    public void LoadLayer(List<Neuron> input)
    {
        totalInputs = input[0].InputNumber;
        totalNeurons = input.Count;
        layerNeurons = input;
    }
}