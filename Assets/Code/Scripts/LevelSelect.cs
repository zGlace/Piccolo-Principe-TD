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

    [SerializeField] private int levelNumber; // The number of this level (e.g., Level 1, Level 2)

    private void Start()
    {
        // Load level progress from PlayerPrefs
        LoadLevelStatus(levelNumber);
    }

    private void Update()
    {
        UpdateLevelUI();
    }

    private void UpdateLevelUI()
    {
        lockedImage.gameObject.SetActive(!unlocked);
        unlockedImage.gameObject.SetActive(unlocked && !completed);
        completedImage.gameObject.SetActive(completed);
    }

    private void LoadLevelStatus(int levelIndex)
    {
        // Check if the previous level is completed (for unlocking purposes)
        if (levelNumber == 1 || PlayerPrefs.GetInt("Level" + (levelNumber - 1) + "_Completed", 0) == 1)
        {
            unlocked = true;
        }
        else
        {
            unlocked = false;
        }

        // Check if this level is completed
        completed = PlayerPrefs.GetInt("Level" + levelNumber + "_Completed", 0) == 1;
    }

    // Called when a level is completed
    public void CompleteLevel()
    {
        PlayerPrefs.SetInt("Level" + levelNumber + "_Completed", 1);
        PlayerPrefs.Save();
        completed = true;
        UpdateLevelUI();
    }

    public void PressSelection(string _LevelName)
    {
        if (unlocked)
        {
            SceneManager.LoadScene(_LevelName); // Only load the level if it's unlocked
        }
    }

    public void ClearPlayerPrefs()
    {
        // Clear all PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Ensure changes are saved

        // Reset level states
        unlocked = false;
        completed = false;

        // Refresh all levels to ensure UI is accurate
        RefreshAllLevels();
    }

    private void RefreshAllLevels()
    {
        // Assuming you have a way to access all level objects
        LevelSelect[] levels = FindObjectsOfType<LevelSelect>();
        foreach (var level in levels)
        {
            level.LoadLevelStatus(level.levelNumber); // Reload each level status
            level.UpdateLevelUI(); // Update the UI for each level
        }
    }
}
