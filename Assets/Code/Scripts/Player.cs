using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	public int maxHealth = 5;
	public int currentHealth;

	public HealthBar healthBar;
    private bool isGameOver = false;

    public void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        
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