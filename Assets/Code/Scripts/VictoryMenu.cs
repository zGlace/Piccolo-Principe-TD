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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GameWon()
    {
        GameFinished = true;
        PauseMenu.GameIsPaused = true;
        StopAllCoroutines();
        Debug.Log("Congratulations! You've completed all the waves.");
    }
}
