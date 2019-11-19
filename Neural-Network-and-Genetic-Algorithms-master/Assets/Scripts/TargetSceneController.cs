using UnityEngine;
using System.Collections.Generic;

public class TargetSceneController : MonoBehaviour 
{   
    void Awake()
    {
        SceneController.targetScene = SceneController.Scene.Menu;
        SceneController.LoadTargetScene();
    }    
}
