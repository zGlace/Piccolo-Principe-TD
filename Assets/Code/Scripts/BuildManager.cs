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
    private BaseTurret turretPreviewScript; // Reference to the Turret component in the preview
    private Vector3 lastPreviewPosition;

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {
        if (PauseMenu.GameIsPaused) // Store the last preview position and do not update its position while paused
        {
            if (towerPreview != null)
            {
                lastPreviewPosition = towerPreview.transform.position;
            }
            return;
        }

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

        // Cancel the preview with right mouse click (1)
        if (Input.GetMouseButtonDown(1) && towerPreview != null)
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

        // Notify the TutorialManager that a turret has been bought
        TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager != null)
        {
            tutorialManager.OnTurretBought();
        }

        // Spawn new tower preview to follow cursor
        if (selectedTower >= 0)
        {
            towerPreview = Instantiate(towers[selectedTower].prefab);
            turretPreviewScript = towerPreview.GetComponent<BaseTurret>();

            // Disable any collider for preview so it doesn't interfere with gameplay
            Collider2D col = towerPreview.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }

            // Show the range circle
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
                turretSlowComponent.ShowRange();
            }
        }
    }

    public void ClearSelectedTower()
    {
        selectedTower = -1;

        // Remove tower preview
        if (towerPreview != null)
        {
            BaseTurret turretComponent = towerPreview.GetComponent<BaseTurret>();
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
