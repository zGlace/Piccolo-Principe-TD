using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tower[] towers;

    public static BuildManager main;

    private int selectedTower = -1;  // -1 means no tower selected
    private GameObject towerPreview; // Tower sprite that will follow the cursor

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

            // Disable the collider for preview so it doesn't interfere with gameplay
            Collider2D col = towerPreview.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }

            towerPreview.GetComponent<Turret>().enabled = false;  // Disable turret shooting behavior during preview
        }
    }

    public void ClearSelectedTower()
    {
        selectedTower = -1;

        // Remove tower preview
        if (towerPreview != null)
        {
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
