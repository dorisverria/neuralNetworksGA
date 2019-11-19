using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform transfrm;
    Transform objective;
    Camera cam;
    Vector2 viewSize;
    float yOffset;
    float offsetTarget;
    float timerTarget;
    float switchToObj;

    public Vector3 Position
    {
        get { return transfrm.position; }
    }

    public void Initialize(Transform objective)
    {
        cam = GetComponent<Camera>();
        //GetComponent<Camera>().orthographic = false;
        transfrm = transform;
        yOffset = transfrm.position.y;
        offsetTarget = transfrm.position.z - objective.position.z;
        timerTarget = 1.0f;
        switchToObj = 0;
        RepositionToObjective(objective);
    }

	public void ManualUpdate()
    {
        Vector3 objPosition = new Vector3(transfrm.position.x, yOffset, objective.position.z + offsetTarget);
        if (timerTarget < 1.0f)
        {
            timerTarget += Time.deltaTime;
            if (timerTarget > 1.0f)
            {
                timerTarget = 1.0f;
            }
            Vector3 direction = objPosition - transfrm.position;
            transfrm.position += direction * (timerTarget / 1.0f);
        }
        else
        {
            transfrm.position = objPosition;
        }       
    }

    void ObjectiveSetup(Transform objTransform)
    {
        objective = objTransform;
        timerTarget = 0;
        switchToObj = 0;
    }

    public void RepositionToObjective(Transform objTransform)
    {
        ObjectiveSetup(objTransform);
        transfrm.position = new Vector3(transfrm.position.x, yOffset, objective.position.z + offsetTarget);
        timerTarget = 1.0f;
    }

    public void SetObjective(Transform objTransform)
    {
        if (objTransform != objective)
        {
            if (switchToObj > 0.1f)
            {
                ObjectiveSetup(objTransform);
            }
            else
            {
                switchToObj += Time.deltaTime;
            }
        }
        else
        {
            switchToObj = 0;
        }
    }
}
