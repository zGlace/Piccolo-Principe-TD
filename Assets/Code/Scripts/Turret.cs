using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class Turret : BaseTurret
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    

    [Header("Attributes")]
    [SerializeField] private float rotationSpeed = 250f;
    [SerializeField] private float bps = 1f; // Bullets per second

    private Transform target;
    private float bpsBase;

    protected override void Start()
    {
        bpsBase = bps;
        base.Start();
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();

        if (!CheckTargetIsInRange())
        {
            target = null;
            return;
        }
        
        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= 1f / bps)
        {
            Shoot();
            timeUntilFire = 0f; // Reset time to zero
        }
    }

    private void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        Transform bestTarget = null;
        int highestPathIndex = -1; // Initial value for comparison
        float closestDistance = Mathf.Infinity; // Initialize to a large value

        foreach (RaycastHit2D hit in hits)
        {
            EnemyMovement enemy = hit.transform.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                int enemyPathIndex = enemy.GetPathIndex();
                
                // Check if this enemy is further along the path
                if (enemyPathIndex > highestPathIndex)
                {
                    highestPathIndex = enemyPathIndex;
                    bestTarget = enemy.transform;
                    closestDistance = Vector2.Distance(transform.position, enemy.transform.position); // Update closest distance as well
                }
                // If the enemy is at the same pathIndex, then check which is closer
                else if (enemyPathIndex == highestPathIndex)
                {
                    float distance = Vector2.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        bestTarget = enemy.transform;
                        closestDistance = distance;
                    }
                }
            }
        }

        // Set the target if a valid one was found
        if (bestTarget != null)
        {
            target = bestTarget;
        }
    }

    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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

        bps = CalculateBPS();
        Debug.Log("New BPS: " + bps);
        Debug.Log("New Range: " + targetingRange);
        Debug.Log("New Cost: " + CalculateCost());
    }

    private float CalculateBPS()
    {
        return bpsBase * Mathf.Pow(level, 0.5f);
    }

    /*
    public void ForceCloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        UIManager.main.SetHoveringState(false);
    }
    */
}