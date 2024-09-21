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
    [SerializeField] private float maxHitPoints = 2;
    [SerializeField] private int currencyWorth = 50;

    private float currentHitPoints;
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

    public void TakeDamage(float dmg)
    {
        enemyHealthUI.SetActive(true);
        
        currentHitPoints -= dmg;

        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHitPoints;
        }

        if (currentHitPoints <= 0 && !isDestroyed)
        {
            EnemySpawner.onEnemyDestroy.Invoke();
            LevelManager.main.IncreaseCurrency(currencyWorth);
            isDestroyed = true;

            Destroy(gameObject);
        }
    }
}
