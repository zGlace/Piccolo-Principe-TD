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
            turret.OpenUpgradeUI();
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

        // Re-enable the collider and turret shooting behavior once placed
        Collider2D col = towerObj.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        turret.enabled = true;  // Enable shooting behavior after placement

        // Clear tower selection after placing
        BuildManager.main.ClearSelectedTower();
    }
}
