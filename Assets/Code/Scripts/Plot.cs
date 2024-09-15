using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    public GameObject towerObj;
    public Turret turret;
    public TurretSlow turretSlow;
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
        if (UIManager.main.IsHoveringUI()) return;

        if (towerObj != null)
        {
            // Open UI for regular turret or slow turret if they exist
            if (turret != null)
            {
                turret.OpenUpgradeUI();
            }
            else if (turretSlow != null)
            {
                Debug.Log("Slow turret clicked - implement upgrade UI if needed");
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
        
        turret = towerObj.GetComponent<Turret>();
        turretSlow = towerObj.GetComponent<TurretSlow>();

        // Re-enable the collider and turret shooting behavior once placed
        Collider2D col = towerObj.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        if (turret != null)
        {
            turret.enabled = true;  // Enable shooting behavior for regular turret
        }
        else if (turretSlow != null)
        {
            turretSlow.enabled = true;  // Enable freezing behavior for slow turret
        }

        // Clear tower selection after placing
        BuildManager.main.ClearSelectedTower();
    }
}
