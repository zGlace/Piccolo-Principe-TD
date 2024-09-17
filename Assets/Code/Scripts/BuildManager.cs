using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tower[] towers; // Array of all possible towers
    [SerializeField] private LayerMask plotLayer;  // Add a layer for valid plots

    public static BuildManager main;

    private int selectedTower = -1;  // -1 means no tower selected
    private GameObject towerPreview; // Tower sprite that will follow the cursor
    private Turret turretPreviewScript; // Reference to the Turret component in the preview

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {
        // If a tower is selected, make its preview follow the cursor
        if (selectedTower >= 0 && towerPreview != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            towerPreview.transform.position = new Vector3(mousePos.x, mousePos.y, 0);

            // Check if the mouse is over a valid plot
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, plotLayer);
            if (hit.collider != null)
            {
                turretPreviewScript.UpdateRangeColor(true);  // Valid plot, set range to green
            }
            else
            {
                turretPreviewScript.UpdateRangeColor(false); // Invalid space, set range to red
            }
        }

        // Cancel the preview with Esc
        if (Input.GetKeyDown(KeyCode.Escape) && towerPreview != null)
        {
            CancelTowerPreview();
        }
    }

    public Tower GetSelectedTower()
    {
        if (selectedTower < 0)
            return null;
        return towers[selectedTower];
    }

    public void SetSelectedTower(int _selectedTower)
    {
        selectedTower = _selectedTower;

        // Remove old tower preview if there was one
        if (towerPreview != null)
        {
            Destroy(towerPreview);
        }

        // Spawn new tower preview to follow cursor
        if (selectedTower >= 0)
        {
            towerPreview = Instantiate(towers[selectedTower].prefab);
            turretPreviewScript = towerPreview.GetComponent<Turret>();

            // Disable any collider for preview so it doesn't interfere with gameplay
            Collider2D col = towerPreview.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }

            // Show the range circle immediately and set its color to red initially
            turretPreviewScript.ShowRange();
            turretPreviewScript.UpdateRangeColor(false);

            // Disable the appropriate shooting or freezing behavior during preview
            Turret turretComponent = towerPreview.GetComponent<Turret>();
            TurretSlow turretSlowComponent = towerPreview.GetComponent<TurretSlow>();

            if (turretComponent != null)
            {
                turretComponent.enabled = false; // Disable regular turret behavior
                turretComponent.ShowRange(); // Show range while dragging
                // turretComponent.ForceCloseUpgradeUI(); // Disable upgrade UI while dragging
            }
            else if (turretSlowComponent != null)
            {
                turretSlowComponent.enabled = false; // Disable freezing turret behavior
            }
        }
    }

    public void ClearSelectedTower()
    {
        selectedTower = -1;

        // Remove tower preview
        if (towerPreview != null)
        {
            Turret turretComponent = towerPreview.GetComponent<Turret>();
            if (turretComponent != null)
            {
                turretComponent.HideRange();  // Hide range when canceling preview
            }
            Destroy(towerPreview);
        }
    }

    // Method to cancel tower preview
    public void CancelTowerPreview()
    {
        ClearSelectedTower();
        Debug.Log("Tower placement canceled.");
    }
}
