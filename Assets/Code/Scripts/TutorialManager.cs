using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager main;
    
    public GameObject[] popups;
    private int popUpIndex = 0;
    public GameObject spawner;
    public float waitTime = 10f;

    private bool turretBought = false; // Flag to check if the player has bought a turret
    private bool turretPlaced = false; // Flag to check if the player has placed a turret
    private bool isReadyToSpawnEnemy = false; // Flag to indicate when to spawn the enemy
    private bool tutorialActive = true; // Flag to indicate if the tutorial is active

    void Awake()
    {
        main = this; // Set the static reference
    }

    void Start()
    {
        // Ensure only the first popup is active at the start
        for (int i = 0; i < popups.Length; i++)
        {
            popups[i].SetActive(i == 0); // Only show the first popup
        }
    }

    void Update()
    {
        // Handle showing the correct popup
        for (int i = 0; i < popups.Length; i++)
        {
            popups[i].SetActive(i == popUpIndex);
        }

        // Handle tutorial progression based on the current popup index
        switch (popUpIndex)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.Space)) // First popup, press Space to continue
                {
                    popUpIndex++;
                }
                break;

            case 1:
                if (Input.GetKeyDown(KeyCode.Space)) // Second popup, press Space to continue
                {
                    popUpIndex++;
                }
                break;

            case 2: // Instruct player to buy a turret
                if (turretBought)  // Move to the next step if the turret is bought
                {
                    popUpIndex++;
                }
                break;

            case 3: // Instruct player to place the turret
                if (turretPlaced)  // Move to the next step if the turret is placed
                {
                    popUpIndex++;
                }
                break;

            case 4: // Final instruction before spawning enemies
                isReadyToSpawnEnemy = true; // Set the flag to indicate readiness
                if (waitTime <= 0)
                {
                    spawner.SetActive(true); // Activate the spawner if needed
                }
                else
                {
                    waitTime -= Time.deltaTime; // Countdown wait time
                }
                break;
            
            case 5: // End tutorial
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    EndTutorial();
                }
                break;
        }
    }

    public int CurrentPopUpIndex()
    {
        return popUpIndex; // Return the current popup index
    }

    public bool IsReadyToSpawnEnemy()
    {
        return isReadyToSpawnEnemy; // Return the flag indicating readiness to spawn
    }

    // Method called when the player buys a turret from the shop
    public void OnTurretBought()
    {
        turretBought = true;
    }

    // Method called when the player places a turret on a deployable tile
    public void OnTurretPlaced()
    {
        turretPlaced = true;
    }

    public void OnEnemyDestroyed()
    {
        // Move to the next tutorial step
        popUpIndex++;
    }

    public bool IsTutorialActive()
    {
        return tutorialActive; // Return the current state of the tutorial
    }

    public void EndTutorial()
    {
        tutorialActive = false; // Set the tutorial state to inactive
        SceneManager.LoadScene("Menu"); // Load the main menu scene
    }
}
