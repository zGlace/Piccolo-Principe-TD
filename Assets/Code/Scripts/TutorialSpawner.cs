using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawner : MonoBehaviour
{
    public GameObject tutorialEnemy; // The enemy prefab to spawn
    public float spawnDelay = 1f; // Delay before spawning the enemy
    private bool enemySpawned = false; // Flag to check if the enemy has been spawned
    private TutorialManager tutorial;

    void Update()
    {
        // Check if the enemy has already been spawned
        if (!enemySpawned)
        {
            // Use the TutorialManager to check if it is time to spawn the enemy
            if (TutorialManager.main.IsReadyToSpawnEnemy()) // Check if we should spawn the enemy
            {
                // Spawn the enemy
                StartCoroutine(SpawnEnemy());
                enemySpawned = true; // Set the flag to true to avoid spawning again
            }
        }
    }

    private IEnumerator SpawnEnemy()
    {
        // Wait for the specified spawn delay
        yield return new WaitForSeconds(spawnDelay);
        
        // Instantiate the enemy at the specified spawn point
        Instantiate(tutorialEnemy, LevelManager.main.startPoint.position, Quaternion.identity);
    }
}
