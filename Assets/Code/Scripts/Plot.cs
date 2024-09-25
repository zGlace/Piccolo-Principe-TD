using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    public GameObject towerObj;
    public BaseTurret baseTurret;
    public Turret turret;
    public TurretSlow turretSlow;
    private Color startColor;

    private void Start()
    {
        if (PauseMenu.GameIsPaused) return; // Prevent hover color change while game is paused
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        if (PauseMenu.GameIsPaused) return;
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
        if (PauseMenu.GameIsPaused || UIManager.main.IsHoveringUI()) return;

        if (towerObj != null)
        {
            // Open UI for regular turret or slow turret
            if (baseTurret != null)
            {
                // Check if the upgrade UI is already active
                if (baseTurret.upgradeUI.activeSelf)
                {
                    baseTurret.CloseUpgradeUI();  // Close the UI if it's already open
                }
                else
                {
                    baseTurret.OpenUpgradeUI();   // Open the UI if it's not open yet
                }
            }
            return;
        }

        Tower towerToBuild = BuildManager.main.GetSelectedTower();
        if (towerToBuild == null) return; // No tower selected

        if (towerToBuild.cost > LevelManager.main.currency)
        {
            Debug.Log("Can't afford this tower");
            return;
        }

        LevelManager.main.SpendCurrency(towerToBuild.cost);

        towerObj = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        
        baseTurret = towerObj.GetComponent<BaseTurret>();

        // Re-enable the collider and turret shooting behavior once placed
        Collider2D col = towerObj.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        if (turret != null)
        {
            turret.enabled = true;  // Enable shooting behavior for regular turret
            baseTurret.HideRange();
            // turret.ForceCloseUpgradeUI();
        }
        else if (turretSlow != null)
        {
            turretSlow.enabled = true;  // Enable freezing behavior for slow turret
            baseTurret.HideRange();
        }

        // Notify the TutorialManager that a turret has been placed
        TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager != null)
        {
            tutorialManager.OnTurretPlaced();  // Inform the tutorial
        }

        // Clear tower selection after placing
        BuildManager.main.ClearSelectedTower();
    }
}
