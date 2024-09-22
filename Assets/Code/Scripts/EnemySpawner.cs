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

    [Header("References")]
    [SerializeField] private TextMeshProUGUI waveText;  // To display wave progress (current wave / max wave)

    [Header("Attributes")]
    [SerializeField] private List<Wave> waves = new List<Wave>();  // List of wave objects, each containing enemies for that wave
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.5f;
    [SerializeField] private float enemiesPerSecondCap = 15f;
    [SerializeField] private int maxWave = 10;  // Maximum number of waves
    [SerializeField] private List<Scene> _sceneList;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();
    public static UnityEvent onEnemyReachedEnd = new UnityEvent();

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private float eps; // Enemies per second
    private bool isSpawning = false;
    public GameObject VictoryCanvas;
    

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
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

    private void EnemyDestroyed()
    {
        enemiesAlive--;
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
        isSpawning = false;
        timeSinceLastSpawn = 0f;

        if (currentWave >= maxWave)
        {
            UnlockLevel();
            EndGame();
        }
        else
        {
            currentWave++;
            UpdateWaveUI();
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
            GameObject prefabToSpawn = currentWaveEnemies[index];
            Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
            Debug.Log("Spawn Enemy");
        }
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(waves[currentWave - 1].enemiesInWave.Length);  // Depends on how many enemies are in the wave
    }

    private float EnemiesPerSecond()
    {
        return Mathf.Clamp(enemiesPerSecond * Mathf.Pow(currentWave, difficultyScalingFactor), 0f, enemiesPerSecondCap);
    }

    /* public void LoadNextScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene < _sceneList.Count)
            SceneManager.LoadScene(_sceneList[currentScene + 1].buildIndex);
        else
            print("Its last scene"); 
    } */
    void UnlockLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel" + 1) + 1);
            PlayerPrefs.Save();
        }
    }

    private void LoadVictory()
    {
        VictoryCanvas.SetActive(true);
    }
    private void EndGame()
    {
        // Stop spawning and trigger the end of the level
        Debug.Log("Congratulations! You've completed all the waves!");
        LoadVictory();
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        // TODO: Implement the logic for moving to the next level
    }

    private void UpdateWaveUI()
    {
        // Update the wave UI display (e.g., "Wave 1/10")
        waveText.text = $"Wave {currentWave}/{maxWave}";
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
