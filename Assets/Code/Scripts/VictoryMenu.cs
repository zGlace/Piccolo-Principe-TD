using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject victoryUI;

    public static bool GameFinished = false;
    
    public void NewLevel()
    {
        GameFinished = false;
        PauseMenu.GameIsPaused = false;
        Debug.Log("PIPPO");
        Time.timeScale = 1f;

        Debug.Log("New Level" + PauseMenu.GameIsPaused);
        if (IsLastLevel())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 4);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // Determine if the current level is the last playable one
    private bool IsLastLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        return currentSceneIndex == SceneManager.sceneCountInBuildSettings - 2; // Tutorial is the last scene, so the last level is second-to-last
    }

    public void GameWon()
    {
        GameFinished = true;
        PauseMenu.GameIsPaused = true;
        StopAllCoroutines();
        Debug.Log("Congratulations! You've completed the level.");
    }
}
