using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject gameOverUI;

    public static bool GameOver = false;

    public void Back()
    {
        GameOver = false;
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
        PauseMenu.GameIsPaused = false;
    }

    public void Retry()
    {
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.ResetSpawner();
        }

        GameOver = false;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Time.timeScale = 1f;
        PauseMenu.GameIsPaused = false;
    }

    public void GameLost()
    {
        GameOver = true;
        PauseMenu.GameIsPaused = true;
        Debug.Log("Game Over!");
    }
}
