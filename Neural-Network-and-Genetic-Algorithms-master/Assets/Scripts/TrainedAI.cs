using UnityEngine;
using System.Collections.Generic;

public class TrainedAI : PlayerMode
{
    Sensors sensor;
    float distProgress;
    [SerializeField]
    List<float> distances;
    [SerializeField]
    float _rightForce;
    [SerializeField]
    float _leftForce;

    public override void Initialize(PlayerType type)
    {
        base.Initialize(type);
        sensor = GetComponent<Sensors>();
        sensor.Initialize();
        distances = new List<float>(sensor.SensorsNumber);
        for (int i = 0; i < sensor.SensorsNumber; i++)
        {
            distances.Add(1);
        }
        distProgress = 0;
        _rightForce = 0;
        _leftForce = 0;
    }

    public override void ManualUpdate()
    {
        if (isLiving)
        {
            sensor.UpdateSensors();
            UpdateInputs();
            Calculate();
            UpdateOutputs();
            CheckWeakPlayers();
            score = rewardsTaken + (int)position.z;
        }
    }

    float Normalize(float value, float maxValue)
    {
        float depth = value / maxValue;
        return 1 - depth;
    }

    void UpdateInputs()
    {
        float length = 45.0f;
        distances[(int)Sensors.SensorPos.L] = Normalize(sensor.GetDistance(Sensors.SensorPos.L), length);
        distances[(int)Sensors.SensorPos.L1] = Normalize(sensor.GetDistance(Sensors.SensorPos.F2), length);
        distances[(int)Sensors.SensorPos.L2] = Normalize(sensor.GetDistance(Sensors.SensorPos.L2), length);
        distances[(int)Sensors.SensorPos.F] = Normalize(sensor.GetDistance(Sensors.SensorPos.F), length);
        distances[(int)Sensors.SensorPos.F1] = Normalize(sensor.GetDistance(Sensors.SensorPos.F1), length);
        distances[(int)Sensors.SensorPos.F2] = Normalize(sensor.GetDistance(Sensors.SensorPos.F2), length);
        distances[(int)Sensors.SensorPos.R] = Normalize(sensor.GetDistance(Sensors.SensorPos.R), length);
    }

    void Calculate()
    {
        _leftForce = 0.0f;
        _rightForce = 0.0f;
        int front = (int)Sensors.SensorPos.F;
        for (int i = 0; i < front; i++)
        {
            if (distances[i] != 1.0f)
            {
                _rightForce += distances[i];
            }
        }
        for (int i = front+1; i < distances.Count; i++)
        {
            if (distances[i] != 1.0f)
            {
                _leftForce += distances[i];
            }
        }        
        if (distances[(int)Sensors.SensorPos.F] != 1.0f)
        {
            if (_leftForce > _rightForce)
            {
                _leftForce += distances[(int)Sensors.SensorPos.F];
            }
            else
            {
                _rightForce += distances[(int)Sensors.SensorPos.F];
            }
        }
        float length = new Vector2(_leftForce, _rightForce).magnitude;
        if (length > 0)
        {
            _leftForce /= length;
            _rightForce /= length;
        }
    }

    void UpdateOutputs()
    {
        float angleChange = (_rightForce - _leftForce) * 180.0f * Time.deltaTime;
        float headingAngle = -trnsfrm.rotation.eulerAngles.y + angleChange;
        float radian = headingAngle * Mathf.PI / 180;
        trnsfrm.eulerAngles += new Vector3(0, angleChange, 0);
        rigidBody.velocity = new Vector3(GameManager.playerSpeed * Mathf.Cos(radian), 0, GameManager.playerSpeed * Mathf.Sin(radian));
    }

    void CheckWeakPlayers()
    {
        if (distProgress > position.z)
        {
            Dead();
        }
        else
        {
            distProgress = position.z;
        }
    }

    public override void Reset()
    {
        base.Reset();
        distProgress = 0.0f;
    }
}
