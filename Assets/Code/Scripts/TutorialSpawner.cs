using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawner : MonoBehaviour
{
    public GameObject tutorialEnemy; // The enemy prefab to spawn
    public float spawnDelay = 1f; // Delay between spawning each enemy
    public int totalEnemies = 3; // Total number of enemies to spawn
    private int enemiesSpawned = 0; // Counter to track how many enemies have been spawned
    private bool spawningEnemies = false; // Flag to check if we're currently spawning enemies
    private TutorialManager tutorial;

    void Update()
    {
        // Check if we need to start spawning enemies
        if (!spawningEnemies && enemiesSpawned < totalEnemies && TutorialManager.main.IsReadyToSpawnEnemy())
        {
            // Set the flag to prevent starting the coroutine multiple times
            spawningEnemies = true;
            
            // Start spawning enemies
            StartCoroutine(SpawnEnemy());
        }
    }

    private IEnumerator SpawnEnemy()
    {
        while (enemiesSpawned < totalEnemies)
        {
            // Instantiate the enemy at the spawn point
            Instantiate(tutorialEnemy, LevelManager.main.startPoint.position, Quaternion.identity);
            
            // Increment the spawned enemies counter
            enemiesSpawned++;

            // Wait for the specified spawn delay before spawning the next enemy
            yield return new WaitForSeconds(spawnDelay);
        }

        // Reset the flag after all enemies have been spawned
        spawningEnemies = false;
    }
}
