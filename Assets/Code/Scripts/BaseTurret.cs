using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class BaseTurret : MonoBehaviour
{
    [Header("Common References")]
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] public GameObject upgradeUI;
    [SerializeField] protected Button upgradeButton;
    [SerializeField] protected TextMeshProUGUI upgradeCostUI;
    [SerializeField] protected GameObject rangeIndicator;
    [SerializeField] protected SpriteRenderer rangeRenderer;

    [Header("Common Attributes")]
    [SerializeField] public float targetingRange = 5f; // Targeting range
    [SerializeField] private int baseUpgradeCost = 100;

    protected float targetingRangeBase;
    protected float timeUntilFire;
    protected int level = 1;
    protected const int maxLevel = 3;

    protected virtual void Start()
    {
        targetingRangeBase = targetingRange;
        upgradeButton.onClick.AddListener(Upgrade);
        UpdateUpgradeCostUI();
    }

    public virtual void OpenUpgradeUI()
    {
        UpdateUpgradeCostUI();
        upgradeUI.SetActive(true);
        ShowRange();
    }

    public virtual void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        UIManager.main.SetHoveringState(false);
        HideRange();
    }

    public virtual void Upgrade()
    {
        // Prevent further upgrades if max level is reached
        if (level >= maxLevel) return;

        LevelManager.main.SpendCurrency(CalculateCost());

        level++;
        targetingRange = CalculateRange();
        UpdateUpgradeCostUI();
        CloseUpgradeUI();
    }

    protected int CalculateCost()
    {
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 1f));
    }

    protected virtual float CalculateRange()
    {
        return targetingRangeBase * Mathf.Pow(level, 0.2f);
    }

    protected void UpdateUpgradeCostUI()
    {
        if (level >= maxLevel)
        {
            upgradeCostUI.text = "MAX";
            upgradeButton.interactable = false;
        }
        else
        {
            int cost = CalculateCost();
            upgradeCostUI.text = cost.ToString();
            upgradeButton.interactable = true;
        }
    }

    public void ShowRange()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);

            float turretScaleFactor = transform.localScale.x; // Assuming uniform scaling
            float scaledRange = targetingRange * 2 / turretScaleFactor; // Multiply by 2 because we want the diameter

            rangeIndicator.transform.localScale = new Vector3(scaledRange, scaledRange, 1);
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
            rangeRenderer.color = isValid ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, targetingRange);
    }
}
