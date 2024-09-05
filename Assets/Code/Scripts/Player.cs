using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	public int maxHealth = 5;
	public int currentHealth;

	public HealthBar healthBar;

    public void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        Debug.Log("Adding listener to onEnemyDestroy");
        EnemySpawner.onEnemyReachedEnd.AddListener(OnEnemyReachedEnd);
    }

    public void OnDestroy()
    {
        EnemySpawner.onEnemyReachedEnd.RemoveListener(OnEnemyReachedEnd);
    }

    public void OnEnemyReachedEnd()
    {
        Debug.Log("Enemy reached end, player takes damage");
        TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Makes it so that the health never reaches negative numbers

        Debug.Log($"Player takes {damage} damage. Current health: {currentHealth}");
        healthBar.SetHealth(currentHealth);

        if (currentHealth == 0)
        {
            Debug.Log("Game Over!");
            QuitGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}