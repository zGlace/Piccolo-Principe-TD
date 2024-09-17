using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] public GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI upgradeCostUI;
    [SerializeField] private GameObject rangeIndicator;
    [SerializeField] private SpriteRenderer rangeRenderer;
    

    [Header("Attributes")]
    [SerializeField] public float targetingRange = 5f; // Distance that the player is able to target enemies
    [SerializeField] private float rotationSpeed = 250f;
    [SerializeField] private float bps = 1f; // Bullets per second
    [SerializeField] private int baseUpgradeCost = 100;

    private float bpsBase;
    private float targetingRangeBase;

    private Transform target;
    private float timeUntilFire;
    private int level = 1;
    private const int maxLevel = 3;

    private void Start()
    {
        bpsBase = bps;
        targetingRangeBase = targetingRange;

        upgradeButton.onClick.AddListener(Upgrade);

        UpdateUpgradeCostUI();
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

        if (hits.Length > 0)
        {
            target = hits[0].transform;
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

    public void Upgrade()
    {
        // Check if turret has reached the max level to prevent further upgrades
        if (level >= maxLevel) return;

        // Check if the player has enough currency
        if (CalculateCost() > LevelManager.main.currency)
        {
            Debug.Log("Can't afford upgrade");
            return;
        }

        LevelManager.main.SpendCurrency(CalculateCost());

        level++;

        bps = CalculateBPS();
        targetingRange = CalculateRange();

        UpdateUpgradeCostUI();
        CloseUpgradeUI();
        Debug.Log("New BPS: " + bps);
        Debug.Log("New Range: " + targetingRange);
        Debug.Log("New Cost: " + CalculateCost());
    }

    private int CalculateCost()
    {
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.8f));
    }

    private float CalculateBPS()
    {
        return bpsBase * Mathf.Pow(level, 0.6f);
    }

    private float CalculateRange()
    {
        return targetingRange * Mathf.Pow(level, 0.4f);
    }

    private void UpdateUpgradeCostUI()
    {
        if (level >= maxLevel)
        {
            upgradeCostUI.text = "MAX"; // Display max level reached message
            upgradeButton.interactable = false; // Disable the upgrade button
        }
        else
        {
            int cost = CalculateCost();
            upgradeCostUI.text = cost.ToString(); // Update the UI with the calculated cost
            upgradeButton.interactable = true; // Ensure the button is interactable if below max level
        }
    }

    public void OpenUpgradeUI()
    {
        UpdateUpgradeCostUI();
        upgradeUI.SetActive(true);
        ShowRange();
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        UIManager.main.SetHoveringState(false);
        HideRange();
    }

    /*
    public void ForceCloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        UIManager.main.SetHoveringState(false);
    }
    */
    
    public void ShowRange()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
            
            // Get the turret's current scale
            float turretScaleFactor = transform.localScale.x; // Assuming uniform scaling

            // Scale the range indicator based on the targetingRange and compensate for the turret's scale
            float scaledRange = targetingRange * 2 / turretScaleFactor;  // Multiply by 2 because we want the diameter
            
            rangeIndicator.transform.localScale = new Vector3(scaledRange, scaledRange, 1);  // Scale only the x and y axes
        }
    }

    public void HideRange()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
        }
    }

    public void UpdateRangeColor(bool isValid)
    {
        if (rangeRenderer != null)
        {
            // Set color to green if valid, red if invalid
            rangeRenderer.color = isValid ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
}
