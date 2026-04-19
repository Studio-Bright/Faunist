using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // optional (keeps it between scenes)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --------------------
    // BASIC LOAD
    // --------------------
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // safety reset
        SceneManager.LoadScene(sceneName);
    }

    // --------------------
    // QUIT
    // --------------------
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}