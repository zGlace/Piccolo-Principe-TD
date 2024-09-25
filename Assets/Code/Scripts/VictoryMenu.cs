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
        
        if (IsLastLevel())
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // Determine if the current level is the last one in the build index
    private bool IsLastLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        return currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1;
    }

    public void GameWon()
    {
        GameFinished = true;
        PauseMenu.GameIsPaused = true;
        StopAllCoroutines();
        Debug.Log("Congratulations! You've completed the level.");
    }
}
