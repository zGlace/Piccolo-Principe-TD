using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [Header("Level State")]
    [SerializeField] private bool unlocked; // Level is unlocked if true
    [SerializeField] private bool completed;

    [Header("UI References")]
    public Image lockedImage; // Image or game object for locked state
    public Image unlockedImage;
    public Image completedImage;

    private string levelKey; // Unique key for PlayerPrefs for this level

    private void Start()
    {
        // Get the level index based on the name or custom logic
        int levelIndex = int.Parse(gameObject.name);

        // Construct a unique key for this level (e.g., "Level1", "Level2", etc.)
        levelKey = "Level" + levelIndex;

        // Load level progress from PlayerPrefs
        LoadLevelStatus(levelIndex);
        
        // Update UI elements based on the level's status
        UpdateLevelUI();
    }

    private void UpdateLevelUI()
    {
        if (!unlocked)
        {
            lockedImage.gameObject.SetActive(true);
            unlockedImage.gameObject.SetActive(false);
            completedImage.gameObject.SetActive(false);
        }
        else if (completed)
        {
            lockedImage.gameObject.SetActive(false);
            unlockedImage.gameObject.SetActive(false);
            completedImage.gameObject.SetActive(true);
        }
        else
        {
            lockedImage.gameObject.SetActive(false);
            unlockedImage.gameObject.SetActive(true);
            completedImage.gameObject.SetActive(false);
        }
    }

    // Load the level's status from PlayerPrefs
    private void LoadLevelStatus(int levelIndex)
    {
        // Check if the level is completed or unlocked
        completed = PlayerPrefs.GetInt(levelKey + "_completed", 0) == 1;
        unlocked = PlayerPrefs.GetInt(levelKey + "_unlocked", 0) == 1;

        // By default, level 1 is unlocked at the start
        if (levelIndex == 1)
        {
            unlocked = true;
        }
        else
        {
            // Unlock the level if the previous one is completed
            string previousLevelKey = "Level" + (levelIndex - 1);
            if (PlayerPrefs.GetInt(previousLevelKey + "_completed", 0) == 1)
            {
                unlocked = true;
                PlayerPrefs.SetInt(levelKey + "_unlocked", 1); // Save unlocked status
            }
        }
    }

    // Called when a level is completed
    public void CompleteLevel()
    {
        completed = true;
        PlayerPrefs.SetInt(levelKey + "_completed", 1); // Mark level as completed
        UpdateLevelUI(); // Update the UI to show completed state
    }

    public void PressSelection(string _LevelName)
    {
        if (unlocked)
        {
            SceneManager.LoadScene(_LevelName); // Only load the level if it's unlocked
        }
    }
}
