using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TurretSlow : BaseTurret
{
    [Header("Attributes")]
    [SerializeField] private float aps = 4f; // Attacks per second
    [SerializeField] private float freezeTime = 1f;

    private float apsBase;
    private float freezeTimeBase;

    protected override void Start()
    {
        apsBase = aps;
        freezeTimeBase = freezeTime;
        base.Start();
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
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                EnemyMovement em = hit.transform.GetComponent<EnemyMovement>();
                em.UpdateSpeed(0.5f);

                StartCoroutine(ResetEnemySpeed(em));
            }
        }
    }

    private IEnumerator ResetEnemySpeed(EnemyMovement em)
    {
        yield return new WaitForSeconds(freezeTime);
        em.ResetSpeed();
    }

    public override void Upgrade()
    {
        // Check if the player has enough currency
        if (CalculateCost() > LevelManager.main.currency)
        {
            Debug.Log("Can't afford upgrade");
            return;
        }

        base.Upgrade();

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
        return freezeTimeBase * Mathf.Pow(level, 0.6f);
    }
}
