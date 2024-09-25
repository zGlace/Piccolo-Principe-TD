using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSlow : BaseTurret
{
    [Header("Attributes")]
    [SerializeField] private float aps = 4f; // Attacks per second
    [SerializeField] private float freezeTime = 1f;
    [SerializeField] private Color slowEffectColor = Color.blue; // Color to apply when enemies are slowed

    private float apsBase;
    private float freezeTimeBase;

    protected override void Start()
    {
        apsBase = aps;
        freezeTimeBase = freezeTime;

        base.Start();
        
        // Always show the targeting range for the slow turret
        ShowRange();
    }

    private void Update()
    {
        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= 1f / aps)
        {
            FreezeEnemies();
            timeUntilFire = 0f; // Reset time to zero
        }
    }

    private void FreezeEnemies()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, Vector2.zero, 0f, enemyMask);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                EnemyMovement em = hit.transform.GetComponent<EnemyMovement>();
                if (em != null)
                {
                    // Slow down the enemy and change its color
                    em.UpdateSpeed(0.5f);
                    StartCoroutine(ApplySlowEffect(em));
                }
            }
        }
    }

    private IEnumerator ApplySlowEffect(EnemyMovement em)
    {
        SpriteRenderer enemyRenderer = em.GetComponent<SpriteRenderer>();
        if (enemyRenderer != null)
        {
            Color originalColor = enemyRenderer.color;
            enemyRenderer.color = slowEffectColor; // Change to slow effect color

            yield return new WaitForSeconds(freezeTime); // Wait for the freeze time

            // Check if the enemy and its sprite renderer still exist before changing the color back
            if (enemyRenderer != null && em != null && em.gameObject != null)
            {
                enemyRenderer.color = originalColor; // Restore the original color
            }
            em.ResetSpeed(); // Reset the enemy's speed
        }
    }

    public override void Upgrade()
    {
        // Check if the player has enough currency
        if (CalculateCost() > LevelManager.main.currency)
        {
            Debug.Log("Can't afford upgrade");
            return;
        }

        if (level >= maxLevel) return;

        LevelManager.main.SpendCurrency(CalculateCost());

        level++;
        targetingRange = CalculateRange();
        UpdateUpgradeCostUI();
        ShowRange();
        CloseUpgradeUI();

        aps = CalculateAPS();
        freezeTime = CalculateFreeze();
        Debug.Log("New APS: " + aps);
        Debug.Log("New FreezeTime: " + freezeTime);
        Debug.Log("New Range: " + targetingRange);
        Debug.Log("New Cost: " + CalculateCost());
    }

    private float CalculateAPS()
    {
        return apsBase * Mathf.Pow(level, 0.5f);
    }

    private float CalculateFreeze()
    {
        return freezeTimeBase * Mathf.Pow(level, 0.4f);
    }

    // Override base class to keep the range always visible
    public override void ShowRange()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
            float turretScaleFactor = transform.localScale.x;
            float scaledRange = targetingRange * 2 / turretScaleFactor;
            rangeIndicator.transform.localScale = new Vector3(scaledRange, scaledRange, 1);
        }
    }

    public override void HideRange()
    {
        // Do nothing to keep the range always visible
    }
}
