using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

public class BossSpawner : MonoBehaviour
{

    [SerializeField] private VolumeController volumeController;
    [SerializeField] private Volume globalVolume;

    [Header("Animation References")]
    [SerializeField] private Animator winTextAnimator;
    [SerializeField] private string victoryAnimation;
    
    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab; // The boss enemy prefab
    private GameObject bossInstance; // Track the boss instance

    [Header("Enemy Mini-Wave Settings")]
    [SerializeField] private GameObject[] miniWaveEnemies; // Enemies that spawn in mini-waves
    [SerializeField] private int enemiesPerMiniWave = 5;
    [SerializeField] private float timeBetweenMiniWaves = 10f;
    [SerializeField] private float enemiesPerSecond = 1f;
    [SerializeField] private float enemiesPerSecondCap = 5f;

    private bool bossSpawned = false;
    private bool isSpawningEnemies = false;
    private bool bossReachedEnd = false;
    private float timeSinceLastSpawn = 0f;
    private int enemiesLeftToSpawn = 0;
    private int enemiesAlive = 0;
    private float eps;
    private VictoryMenu victory;

    private void Awake()
    {
        victory = FindObjectOfType<VictoryMenu>();
        
        LevelManager.onEnemyDestroy.AddListener(OnEnemyDestroyed);
        LevelManager.onBossDefeated.AddListener(victory.GameWon);
        StartCoroutine(StartBoss());

    }
    
    private IEnumerator StartBoss()
    {
        yield return new WaitForSeconds(5f); // Wait 5 seconds at the start of the game before commencing the waves
        SpawnBoss();
        volumeController.SetGlobalVolume(globalVolume);
        volumeController.ModifyVignette(Color.magenta, 0.3f, 1.0f, float.PositiveInfinity, 1.0f);
        volumeController.ModifyBloom(18.0f, 1.0f, float.PositiveInfinity, 1.0f);
        volumeController.ModifyChromaticAberration(0.1f, 1.0f, float.PositiveInfinity, 1.0f);
    }

    private void Update()
    {
        if (bossInstance == null && bossSpawned && !VictoryMenu.GameFinished && !bossReachedEnd)
        {
            if (VictoryMenu.GameFinished) return;  // Prevent multiple calls
            
            isSpawningEnemies = false;  // Stop spawning enemies

            victory.victoryUI.SetActive(true);
            winTextAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            winTextAnimator.Play(victoryAnimation, 0, 0.0f);
            DestroyAllEnemies();
            victory.GameWon();

            PlayerPrefs.SetInt("Level" + SceneManager.GetActiveScene().buildIndex + "_Completed", 1);
            PlayerPrefs.Save();
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
        LevelManager.onBossDefeated.RemoveListener(victory.GameWon);

    }

    public void BossReachedEnd()
    {
        bossReachedEnd = true; // Set this flag to indicate that the boss reached the end
    }

    public void OnBossDefeated()
    {
        // Call this method when the boss is defeated
        if (bossInstance != null)
        {
            Destroy(bossInstance);
            // victory.GameWon();
        }
    }

    public void DestroyAllEnemies()
    {
        // Trova tutti gli oggetti attivi nella scena
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Scorri tutti gli oggetti e controlla se sono nel layer "Enemy"
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("Enemy"))
            {
                Destroy(obj);  // Distruggi l'oggetto
            }
        }

        Debug.Log("Tutti gli elementi nel layer Enemy sono stati distrutti.");
    }
}
