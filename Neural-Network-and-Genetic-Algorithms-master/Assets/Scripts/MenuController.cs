using UnityEngine;

public class MenuController : MonoBehaviour
{
    bool transition;
    [SerializeField]
    SceneSwitcher _transitionPanel;

    void Awake()
    {
        transition = false;
        _transitionPanel.Initialize();
    }

    void Update()
    {
        _transitionPanel.ManualUpdate();

        if (transition)
        {
            if (!_transitionPanel.playingAnimation)
            {
                SceneController.LoadTargetScene();
            }
        }
    }

    public void ArtificialIntelligenceBattle()
    {
        SetupSelectedMode(GameManager.GameMode.AI_vs_AI);
        ChangeToGameScene();
    }

    public void NeuralNetworkTraining()
    {
        SetupSelectedMode(GameManager.GameMode.NeuralNetworkTraining);
        ChangeToGameScene();
    }

    void ChangeToGameScene()
    {
        _transitionPanel.Appear();
        SceneController.targetScene = SceneController.Scene.Game;
        transition = true;
    }

    void SetupSelectedMode(GameManager.GameMode mode)
    {
        switch (mode)
        {
            case GameManager.GameMode.AI_vs_AI:
                SetupAIBattle(mode);
                break;
            case GameManager.GameMode.NeuralNetworkTraining:
                SetupNeuralNetworkTraining(mode);
                break;

            default:
                break;
        }
    }

    void SetupForFreeForAll(GameManager.GameMode mode)
    {
        GameManager.gameMode = mode;
    }

    void SetupAIBattle(GameManager.GameMode mode)
    {
        GameManager.gameMode = mode;
    }

    void SetupNeuralNetworkTraining(GameManager.GameMode mode)
    {
        GameManager.gameMode = mode;
    }

    public void Quit()
    {
        SceneController.QuitGame();
    }
}
