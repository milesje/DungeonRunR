using UnityEngine;

public class StartMenu : MonoBehaviour
{

    public void GameStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("PCG_Game");
    }

    public void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
