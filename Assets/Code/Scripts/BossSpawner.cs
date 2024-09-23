using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab; // The boss enemy prefab
    private GameObject bossInstance; // Track the boss instance

    [Header("Enemy Mini-Wave Settings")]
    [SerializeField] private GameObject[] miniWaveEnemies; // Enemies that spawn in mini-waves
    [SerializeField] private int enemiesPerMiniWave = 5;
    [SerializeField] private float timeBetweenMiniWaves = 10f;
    [SerializeField] private float enemiesPerSecond = 1f;
    [SerializeField] private float enemiesPerSecondCap = 5f;

    [Header("Post-Processing Settings")]
    [SerializeField] private VolumeController volumeController; // Reference to VolumeController
    [SerializeField] private float bloomIntensityDuringBoss = 45f;
    [SerializeField] private float chromaticAberrationIntensityDuringBoss = 0.1f;
    [SerializeField] private Color vignetteColorDuringBoss = Color.magenta; // Optional if you want to change Vignette

    private bool bossSpawned = false;
    private bool levelEnded = false;
    private bool isSpawningEnemies = false;
    private float timeSinceLastSpawn = 0f;
    private int enemiesLeftToSpawn = 0;
    private int enemiesAlive = 0;
    private float eps;

    private void Awake()
    {
        LevelManager.onEnemyDestroy.AddListener(OnEnemyDestroyed);
        LevelManager.onBossDefeated.AddListener(GameWon);
        StartCoroutine(StartBoss());
    }

    private IEnumerator StartBoss()
    {
        yield return new WaitForSeconds(5f); // Wait 5 seconds at the start of the game before commencing the waves
        SpawnBoss();
    }

    private void Update()
    {
        if (bossInstance == null && bossSpawned && !levelEnded)
        {
            GameWon();
        }

        // If spawning enemies, handle timing and enemy spawning logic
        if (isSpawningEnemies)
        {
            timeSinceLastSpawn += Time.deltaTime;

            // Spawn enemies at intervals based on enemiesPerSecond
            if (timeSinceLastSpawn >= (1f / eps) && enemiesLeftToSpawn > 0)
            {
                SpawnEnemy();
                enemiesLeftToSpawn--;
                enemiesAlive++;
                timeSinceLastSpawn = 0f;
            }
        }
    }

    private void SpawnBoss()
    {
        bossInstance = Instantiate(bossPrefab, LevelManager.main.startPoint.position, Quaternion.identity);
        bossSpawned = true;
        enemiesAlive++; // Increment enemiesAlive to account for the boss
        Debug.Log("Boss Spawned");

        // Modifica gli effetti post-processo quando il boss spawna
        if (volumeController != null)
        {
            volumeController.ModifyBloom(bloomIntensityDuringBoss, 1f, Mathf.Infinity, 1f); // Incrementa il bloom
            volumeController.ModifyChromaticAberration(chromaticAberrationIntensityDuringBoss, 1f, Mathf.Infinity, 1f); // Incrementa l'aberration
            volumeController.ModifyVignette(vignetteColorDuringBoss, 0.3f, 1f, Mathf.Infinity, 1f); // Cambia il colore della vignette (opzionale)
        }

        // Start the periodic mini-waves after boss spawns
        StartCoroutine(SpawnMiniWaves());
    }

    private IEnumerator SpawnMiniWaves()
    {
        isSpawningEnemies = true;

        // Keep spawning mini-waves until the boss is defeated
        while (bossInstance != null)
        {
            StartMiniWave();
            yield return new WaitForSeconds(timeBetweenMiniWaves); // Wait for the defined time between waves
        }

        isSpawningEnemies = false;
    }

    private void StartMiniWave()
    {
        enemiesLeftToSpawn = enemiesPerMiniWave; // Set the number of enemies to spawn in the mini-wave
        eps = Mathf.Clamp(enemiesPerSecond, 0f, enemiesPerSecondCap); // Set the spawn rate, clamped by the cap
    }

    private void SpawnEnemy()
    {
        if (enemiesLeftToSpawn > 0)
        {
            // Randomly pick an enemy type from the mini-wave
            GameObject enemyPrefab = miniWaveEnemies[Random.Range(0, miniWaveEnemies.Length)];

            Instantiate(enemyPrefab, LevelManager.main.startPoint.position, Quaternion.identity);
            Debug.Log($"{enemyPrefab.name} Spawned.");
        }
    }

    public void OnEnemyDestroyed()
    {
        enemiesAlive--; // Decrement enemiesAlive when an enemy (or boss) is destroyed
    }

    private void OnDestroy()
    {
        LevelManager.onEnemyDestroy.RemoveListener(OnEnemyDestroyed);
        LevelManager.onBossDefeated.RemoveListener(GameWon);
    }

    public void OnBossDefeated()
    {
        // Call this method when the boss is defeated
        if (bossInstance != null)
        {
            // Ripristina gli effetti post-processo ai valori originali
            if (volumeController != null)
            {
                volumeController.ModifyBloom(0f, 1f, 1f, 1f); // Ripristina il bloom
                volumeController.ModifyChromaticAberration(0f, 1f, 1f, 1f); // Ripristina l'aberration
                volumeController.ModifyVignette(Color.black, 0f, 1f, 1f, 1f); // Ripristina la vignette (opzionale)
            }

            Destroy(bossInstance);
            GameWon();

        }
    }

    private void GameWon()
    {
        if (levelEnded) return;  // Prevent multiple calls
        levelEnded = true; // Set the flag so it won't be called again

        Debug.Log("Level Complete! The boss is defeated.");
        StopAllCoroutines(); // Stop spawning mini-waves
        isSpawningEnemies = false;  // Stop spawning enemies


    }
}
