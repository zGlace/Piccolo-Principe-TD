using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{

    [System.Serializable]
    public class Wave
    {
        public GameObject[] enemiesInWave;  // Enemies that spawn during this wave
    }

    [Header("Animation References")]
    [SerializeField] private TextMeshProUGUI waveText;  // To display wave progress (current wave / max wave)
    [SerializeField] private Animator waveTextAnimator;
    [SerializeField] private string wavePulseAnimation;
    [SerializeField] private Animator winTextAnimator;
    [SerializeField] private string victoryAnimation;

    [Header("Attributes")]
    [SerializeField] private List<Wave> waves = new List<Wave>();  // List of wave objects, each containing enemies for that wave
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.5f;
    [SerializeField] private float enemiesPerSecondCap = 15f;
    [SerializeField] private int maxWave = 10;  // Maximum number of waves

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private float eps; // Enemies per second
    private bool isSpawning = false;
    private VictoryMenu victory;

    private void Awake()
    {
        LevelManager.onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
        victory = FindObjectOfType<VictoryMenu>(); // Automatically find the VictoryMenu object (make sure there's only one in the scene)
        UpdateWaveUI();
        StartCoroutine(StartWave());
    }

    private void Update()
    {
        if (!isSpawning) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / eps) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if (enemiesAlive == 0 && enemiesLeftToSpawn == 0)
        {
            EndWave();
        }
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
        eps = EnemiesPerSecond();
    }

    private void EndWave()
    {
        if (LoseMenu.GameOver) return; // Don't progress waves if the player has lost

        isSpawning = false;
        timeSinceLastSpawn = 0f;

        if (currentWave >= maxWave) // Stop spawning and trigger the end of the level
        {
            victory.victoryUI.SetActive(true);
            winTextAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            winTextAnimator.Play(victoryAnimation, 0, 0.0f);
            // victory.GameWon();
            
            PlayerPrefs.SetInt("Level" + SceneManager.GetActiveScene().buildIndex + "_Completed", 1);
            PlayerPrefs.Save();
        }
        else
        {
            currentWave++;
            UpdateWaveUI();
            if (waveTextAnimator != null)
            {
                waveTextAnimator.Play(wavePulseAnimation, 0, 0.0f);
            }
            StartCoroutine(StartWave());
        }
    }

    private void SpawnEnemy()
    {
        if (currentWave - 1 >= waves.Count) return;  // Safety check if currentWave exceeds list count
        
        // Access the list of enemies specific to the current wave
        GameObject[] currentWaveEnemies = waves[currentWave - 1].enemiesInWave; 

        if (currentWaveEnemies.Length > 0)
        {
            int index = Random.Range(0, currentWaveEnemies.Length);  // Randomly pick from that wave's enemies
            GameObject prefabToSpawn = currentWaveEnemies[index];  // Declare prefabToSpawn here
            
            if (prefabToSpawn != null)  // Check if prefab is not null
            {
                // Ensure LevelManager and startPoint exist
                if (LevelManager.main != null && LevelManager.main.startPoint != null)
                {
                    // Instantiate the enemy at the start point position
                    Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
                    Debug.Log($"{prefabToSpawn.name} Spawned");
                }
                else
                {
                    Debug.LogWarning("LevelManager or startPoint is missing.");
                }
            }
            else
            {
                Debug.LogWarning("Attempted to spawn a null prefab.");
            }
        }
    }

    public void ResetSpawner()
    {
        currentWave = 1; // Reset wave to 1
        timeSinceLastSpawn = 0f;
        enemiesAlive = 0;
        enemiesLeftToSpawn = 0;
        isSpawning = false;
        StopAllCoroutines(); // Stop any existing wave coroutines

        UpdateWaveUI();
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(waves[currentWave - 1].enemiesInWave.Length);  // Depends on how many enemies are in the wave
    }

    private float EnemiesPerSecond()
    {
        return Mathf.Clamp(enemiesPerSecond * Mathf.Pow(currentWave, difficultyScalingFactor), 0f, enemiesPerSecondCap);
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    private void OnDestroy()
    {
        LevelManager.onEnemyDestroy.RemoveListener(EnemyDestroyed);
    }

    private void UpdateWaveUI()
    {
        // Update the wave UI display (e.g., "Wave 1/10")
        waveText.text = $"{currentWave}/{maxWave}";
    }

    private void OnValidate()
    {
        // Ensure that the waves array is always the same size as maxWave
        if (waves.Count > maxWave)
        {
            // Remove excess waves if the count is greater than maxWave
            waves.RemoveRange(maxWave, waves.Count - maxWave);
        }
        else if (waves.Count < maxWave)
        {
            // Add empty waves if the count is less than maxWave
            while (waves.Count < maxWave)
            {
                waves.Add(new Wave());
            }
        }
    }
}
