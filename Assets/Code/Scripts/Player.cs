using UnityEngine.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    private VignetteController vignetteController;
    public int maxHealth = 5;
    public int currentHealth;

    public HealthBar healthBar;
    private bool isGameOver = false;

    public Volume globalVolume;  // Assicurati di assegnare questo nell'Inspector

    public void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Ottieni il riferimento al VignetteController
        vignetteController = GetComponent<VignetteController>();

        // Passa il Global Volume al VignetteController
        if (vignetteController != null && globalVolume != null)
        {
            vignetteController.SetGlobalVolume(globalVolume);
        }

        LevelManager.onEnemyReachedEnd.AddListener(OnEnemyReachedEnd);
    }

    public void OnDestroy()
    {
        LevelManager.onEnemyReachedEnd.RemoveListener(OnEnemyReachedEnd);
    }

    public void OnEnemyReachedEnd()
    {
        if (!isGameOver) // Only take damage if the game is not over
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (vignetteController != null)
        {
            Debug.Log("Colpito!");
            vignetteController.ModifyVignette(Color.red, 0.3f, 0.15f, 0.6f, 0.7f);
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Prevent negative health

        Debug.Log($"Player takes {damage} damage. Current health: {currentHealth}");
        healthBar.SetHealth(currentHealth);

        if (currentHealth == 0 && !isGameOver)
        {
            isGameOver = true;
            Debug.Log("Game Over!");
            QuitGame();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#else
            Application.Quit(); // Quit application
#endif
    }
}
