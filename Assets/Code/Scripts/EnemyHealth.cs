using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject enemyHealthUI;
    [SerializeField] private Slider healthBarSlider;
    
    [Header("Attributes")]
    [SerializeField] public float maxHitPoints = 2;
    [SerializeField] private int currencyWorth = 50;

    [HideInInspector]
    public float currentHitPoints;

    private bool isDestroyed = false;

    private void Start()
    {
        currentHitPoints = maxHitPoints;

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHitPoints;
            healthBarSlider.value = maxHitPoints;
        }
    }

    public void Heal(float healAmount)
    {
        if (isDestroyed) return; // Don't heal if the enemy is destroyed

        currentHitPoints += healAmount;
        if (currentHitPoints > maxHitPoints)
        {
            currentHitPoints = maxHitPoints; // Cap the HP at max
        }
        healthBarSlider.value = currentHitPoints;
    }

    public void EnemyTakeDamage(float dmg)
    {
        enemyHealthUI.SetActive(true);
        
        currentHitPoints -= dmg;

        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHitPoints;
        }

        if (currentHitPoints <= 0 && !isDestroyed)
        {
            LevelManager.onEnemyDestroy.Invoke();
            LevelManager.main.IncreaseCurrency(currencyWorth);
            isDestroyed = true;

            // Notify the tutorial manager about enemy destruction
            TutorialManager.main.OnEnemyDestroyed();

            Destroy(gameObject);
        }
    }
}
