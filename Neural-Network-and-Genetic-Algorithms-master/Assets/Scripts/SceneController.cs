using UnityEngine.SceneManagement;

public class SceneController 
{
    public enum Scene
	{
		Scene0,
		Menu,
		Game,
	}
    public static Scene currentScene = Scene.Scene0;
    public static Scene targetScene = currentScene;

    static public void LoadTargetScene()
    {
        switch (targetScene)
        {
            case Scene.Menu:
                GoToMenu();
                break;

            case Scene.Game:
                GoToGame();
                break;
        }

        currentScene = targetScene;
    }

    static public void GoToMenu()
	{
		SceneManager.LoadScene ((int)Scene.Menu);
        currentScene = Scene.Menu;
        targetScene = currentScene;
    }

    static public void GoToGame()
	{
		SceneManager.LoadScene ((int)Scene.Game);
        currentScene = Scene.Game;
        targetScene = currentScene;
    }

    static public void QuitGame()
	{
        UnityEngine.Application.Quit();
	}
}
