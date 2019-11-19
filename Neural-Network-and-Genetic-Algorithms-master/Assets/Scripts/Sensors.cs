using UnityEngine;
using System.Collections.Generic;

public class Sensors : MonoBehaviour
{
    public enum SensorPos
    {
        L,
        L1,
        L2,
        F,
        F1,
        F2,
        R,
        Count
    }
    public struct SensorInfo
    {
        public Vector3 coordinates;
        public float distance;
    }
    Transform tranfrm;
    SensorInfo[] sensorInfo;
    [SerializeField]
    LayerMask _layerMask;
    [SerializeField]
    Transform castFrom;

    public float GetDistance(SensorPos direction)
    {
        return sensorInfo[(int)direction].distance;
    }

    public int SensorsNumber
    {
        get { return (int)SensorPos.Count; }
    }

    float CosX(float radian, float length)
    {
        return Mathf.Cos(radian) * length;
    }

    float SinX(float radian, float length)
    {
        return Mathf.Sin(radian) * length;
    }

    float GetAngle(float pi)
    {
        return Mathf.PI * pi;
    }

    public void Initialize()
    {
        tranfrm = transform;
        int size = (int)SensorPos.Count;
        sensorInfo = new SensorInfo[size];
        for (int i = 0; i < size; i++)
        {
            sensorInfo[i].distance = 0;
        }
        CalculateDirections();
    }

    void CalculateDirections()
    {
        const float length = 45.0f;
        float angle = -tranfrm.rotation.eulerAngles.y * Mathf.PI / 180;
        float radian = angle;
        radian = angle;
        sensorInfo[(int)SensorPos.F].coordinates = new Vector3(CosX(radian, length), 0, SinX(radian, length));
        radian = angle - GetAngle(0.5f);
        sensorInfo[(int)SensorPos.R].coordinates = new Vector3(CosX(radian, length), 0, SinX(radian, length));
        radian = angle - GetAngle(0.25f);
        sensorInfo[(int)SensorPos.F2].coordinates = new Vector3(CosX(radian, length), 0, SinX(radian, length));
        radian = angle - GetAngle(0.04175f);
        sensorInfo[(int)SensorPos.F1].coordinates = new Vector3(CosX(radian, length), 0, SinX(radian, length));
        radian = angle + GetAngle(0.04175f);
        sensorInfo[(int)SensorPos.L2].coordinates = new Vector3(CosX(radian, length), 0, SinX(radian, length));
        radian = angle + GetAngle(0.25f);
        sensorInfo[(int)SensorPos.L1].coordinates = new Vector3(CosX(radian, length), 0, SinX(radian, length));
        radian = angle + GetAngle(0.5f);
        sensorInfo[(int)SensorPos.L].coordinates = new Vector3(CosX(radian, length), 0, SinX(radian, length));
    }

    public void UpdateSensors()
    {
        CalculateDirections();
        EmitSensors();
    }

    void SetRaycastData(ref RaycastHit hit, int index)
    {
        if (hit.distance != 0)
        {
            Debug.DrawLine(castFrom.position, hit.point);
        }
        sensorInfo[index].distance = hit.distance;
    }

    void EmitSensor(SensorPos direction)
    {
        int index = (int)direction;
        RaycastHit hit;
        Physics.Raycast(castFrom.position, sensorInfo[index].coordinates, out hit, 45.0f);
        SetRaycastData(ref hit, index);
    }

    void EmitSensors()
    {
        EmitSensor(SensorPos.R);
        EmitSensor(SensorPos.F2);
        EmitSensor(SensorPos.F1);
        EmitSensor(SensorPos.F);
        EmitSensor(SensorPos.L2);
        EmitSensor(SensorPos.L1);
        EmitSensor(SensorPos.L);
    }
}
