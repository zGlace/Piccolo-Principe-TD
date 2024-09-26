using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject gameOverUI;

    [Header("Animation References")]
    [SerializeField] private Animator loseTextAnimator;
    [SerializeField] private string loseAnimation;

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
        PauseMenu.GameIsPaused = true; // TODO check
        gameOverUI.SetActive(true);
        loseTextAnimator.updateMode = AnimatorUpdateMode.UnscaledTime; // Set Animator to Unscaled Time so animation plays even when time is frozen
        loseTextAnimator.Play(loseAnimation, 0, 0.0f);
        Time.timeScale = 0f;
        Debug.Log("Game Over!");
    }
}
