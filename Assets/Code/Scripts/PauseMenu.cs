using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI; // The entire pause menu
    public GameObject pauseSelectionUI; // The default screen with the resume/option buttons
    public GameObject optionsMenuUI; // The options menu

    public static bool GameIsPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        ResetToPauseSelection(); // Make sure the main pause screen is shown first

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    // Reset the UI to always show the "Pause Selection" screen when paused
    private void ResetToPauseSelection()
    {
        pauseSelectionUI.SetActive(true); // Show the main pause screen
        optionsMenuUI.SetActive(false); // Hide the options menu
    }

    public void OpenOptionsMenu()
    {
        pauseSelectionUI.SetActive(false); // Hide the main pause screen
        optionsMenuUI.SetActive(true); // Show the options menu
    }

    public void CloseOptionsMenu()
    {
        optionsMenuUI.SetActive(false); // Hide the options menu
        pauseSelectionUI.SetActive(true); // Show the main pause screen
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        PauseMenu.GameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }
}
