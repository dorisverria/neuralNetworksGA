using UnityEngine;
using System.Collections.Generic;

public class NeuralNetworkPlayer : PlayerMode
{
    Sensors sensor;
    NeuralNetwork neuralNetwork;
    [SerializeField]
    float _lifeSpan;
    [SerializeField]
    float distance;
    [SerializeField]
    float _fitness;

    public override void Initialize(PlayerType type)
    {
        base.Initialize(type);
        sensor = GetComponent<Sensors>();
        neuralNetwork = new NeuralNetwork();
        sensor.Initialize();
        Reset();
    }

    public void InitializeNeuralNetwork(NeuralNetwork neuralNetwork)
    {
        this.neuralNetwork = neuralNetwork;
    }

    public override void ManualUpdate()
    {
        if (isLiving)
        {
            sensor.UpdateSensors();
            UpdateNeuralNetwork();
            score = rewardsTaken + (int)position.z;
        }
    }

    void UpdateNeuralNetwork()
    {
        UpdateInputs();
        UpdateOutputs();
        UpdateFitness();
        CheckForWeakAgents();
    }

    void UpdateInputs()
    {
        List<float> inputs = new List<float>(neuralNetwork.Inputs.Count);
        float length = 45.0f;
        inputs.Add(Normalise(sensor.GetDistance(Sensors.SensorPos.L), length));
        inputs.Add(Normalise(sensor.GetDistance(Sensors.SensorPos.L1), length));
        inputs.Add(Normalise(sensor.GetDistance(Sensors.SensorPos.L2), length));
        inputs.Add(Normalise(sensor.GetDistance(Sensors.SensorPos.F), length));
        inputs.Add(Normalise(sensor.GetDistance(Sensors.SensorPos.F1), length));
        inputs.Add(Normalise(sensor.GetDistance(Sensors.SensorPos.F2), length));
        inputs.Add(Normalise(sensor.GetDistance(Sensors.SensorPos.R), length));
        neuralNetwork.Inputs = inputs;
        neuralNetwork.Calculate();
    }

    void UpdateOutputs()
    {
        List<float> outputs = neuralNetwork.Outputs;
        float rightForce = outputs[0];
        float leftForce = outputs[1];
        float angleChange = (rightForce - leftForce) * 180.0f * Time.deltaTime;
        float headingAngle = -trnsfrm.rotation.eulerAngles.y + angleChange;
        float radian = headingAngle * Mathf.PI / 180;
        trnsfrm.eulerAngles += new Vector3(0, angleChange, 0);
        rigidBody.velocity = new Vector3(GameManager.playerSpeed * Mathf.Cos(radian), 0, GameManager.playerSpeed * Mathf.Sin(radian));
    }

    void UpdateFitness()
    {
        _lifeSpan += Time.deltaTime;
        _fitness = _lifeSpan;
    }

    void CheckForWeakAgents()
    {
        if (distance > position.z)
        {
            Dead();
            _fitness = 0;
        }
        else
        {
            distance = position.z;
        }
    }

    float Normalise(float value, float maxValue)
    {
        float depth = value / maxValue;
        return 1 - depth;
    }

    float Clamp(float val, float min, float max)
    {
        if (val < min)
        {
            val = min;
        }
        else if (val > max)
        {
            val = max;
        }

        return val;
    }

    public override void Reset()
    {
        base.Reset();
        distance = 0.0f;
        _fitness = 0.0f;
        _lifeSpan = 0.0f;
    }

    public float fitness
    {
        get { return _fitness; }
    }
}
