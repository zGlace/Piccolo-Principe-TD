using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	public int maxHealth = 5;
	public int currentHealth;

	public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        EnemySpawner.onEnemyDestroy.AddListener(OnEnemyReachedEnd);
    }

    void OnDestroy()
    {
        EnemySpawner.onEnemyDestroy.RemoveListener(OnEnemyReachedEnd);
    }

    void OnEnemyReachedEnd()
    {
        TakeDamage(1);
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Makes it so that the health never reaches negative numbers

        healthBar.SetHealth(currentHealth);

        if (currentHealth == 0)
        {
            // To do: Game Over
            Debug.Log("Game Over!");
        }
    }
}