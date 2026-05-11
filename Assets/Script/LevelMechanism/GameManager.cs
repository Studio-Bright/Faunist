using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public DayManager dayManager;
    public AnimalEncounterManager encounterManager;

    public GameObject pauseMenuUI;
    public GameObject settingsUI;
    public GameObject cursorCanvas;

    private bool isPaused = false;
    public static bool IsPaused;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TogglePause();
        }

        
    }

    // --------------------
    // PAUSE LOGIC
    // --------------------
    public void TogglePause()
    {
        if (!isPaused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    public void Pause()
    {
        Debug.Log("Paused");
        pauseMenuUI.SetActive(true);
        cursorCanvas.SetActive(false);
        Time.timeScale = 0f;

        isPaused = true;
        IsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        Debug.Log("Resume clicked");
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(false);
        cursorCanvas.SetActive(true);
        Time.timeScale = 1f;

        isPaused = false;
        IsPaused = false;

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

    public void LoadMainMenu()
    {
        Debug.Log("Main menu opened");
        //SceneManager.LoadScene("MainMenu"); 
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}