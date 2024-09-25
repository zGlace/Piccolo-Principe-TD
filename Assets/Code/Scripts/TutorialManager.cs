using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] popUps;
    private int popUpIndex = 0;
    public GameObject spawner;
    public float waitTime = 10f;

    private bool turretBought = false;  // Flag to check if the player has bought a turret
    private bool turretPlaced = false;  // Flag to check if the player has placed a turret

    void Update()
    {
        // Handle showing the correct popup
        for (int i = 0; i < popUps.Length; i++)
        {
            if (i == popUpIndex)
            {
                popUps[popUpIndex].SetActive(true);
            }
            else
            {
                popUps[popUpIndex].SetActive(false);
            }
        }

        // Handle tutorial progression based on the current popup index
        if (popUpIndex == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Introductory text, press Space to continue
            {
                popUpIndex++;
            }
            else if (popUpIndex == 1)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    popUpIndex++;
                }
            }
            else if (popUpIndex == 2) // Instruct player to buy a turret
            {
                if (turretBought)  // Move to the next step if the turret is bought
                {
                    popUpIndex++;
                }
            }
            else if (popUpIndex == 3) // Instruct player to place the turret
            {
                if (turretPlaced)  // Move to the next step if the turret is placed
                {
                    popUpIndex++;
                }
            }
            else if (popUpIndex == 4)
            {
                if (waitTime <= 0)
                {
                    spawner.SetActive(true);
                }
                else
                {
                    waitTime -= Time.deltaTime;
                }
            }
        }
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
}

public class Spawner : MonoBehaviour
{
    public GameObject tutorialEnemy;

    public float startTimeBtwEnemy;
    private float timeBtwEnemy;

    public int numberOfEnemies;

    void Update()
    {
        if (timeBtwEnemy <= 0 && numberOfEnemies > 0)
        {
            Instantiate(tutorialEnemy, LevelManager.main.startPoint.position, Quaternion.identity);
            timeBtwEnemy = startTimeBtwEnemy;
            numberOfEnemies++;
        }
        else
        {
            timeBtwEnemy -= Time.deltaTime;
        }
    }
}