using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject settingsUI;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsUI.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                TogglePause();
            }
        }


    }// --------------------
    // PAUSE LOGIC
    // --------------------
    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        Debug.Log("Paused");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // 🔓 Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        Debug.Log("Resume clicked");
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // --------------------
    // SETTINGS
    // --------------------
    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    // --------------------
    // OTHER BUTTONS
    // --------------------
    public void LoadMainMenu()
    {
        Debug.Log("Main menu opened");
        //SceneManager.LoadScene("MainMenu"); // change to your scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
