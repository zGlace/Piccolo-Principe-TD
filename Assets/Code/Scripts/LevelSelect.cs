using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for Pointer events

public class LevelSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Level State")]
    [SerializeField] private bool unlocked; // Level is unlocked if true
    [SerializeField] private bool completed;

    [Header("UI References")]
    public Image lockedImage; // Image for locked state
    public Image unlockedImage; // Image for unlocked state
    public Image completedImage; // Image for completed state
    public Button button; // The button handling clicks

    [SerializeField] private int levelNumber; // The number of this level (e.g., Level 1, Level 2)

    // Define the original and hover colors
    private Color normalColor = Color.white;
    private Color hoverColor = new Color(0.7f, 0.7f, 0.7f);

    private void Start()
    {
        PauseMenu.GameIsPaused = false;
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

    // Implement pointer enter and exit to change color on hover for unlocked and completed levels
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only allow hover color change if the level is unlocked or completed
        if (unlocked || completed)
        {
            SetHoverColor(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Only allow hover color change if the level is unlocked or completed
        if (unlocked || completed)
        {
            SetHoverColor(false);
        }
    }

    // Change the button color based on hover state
    private void SetHoverColor(bool isHovering)
    {
        Color targetColor = isHovering ? hoverColor : normalColor;

        // Change the color only for the active image (unlocked or completed)
        if (unlockedImage.gameObject.activeSelf)
        {
            unlockedImage.color = targetColor;
        }
        else if (completedImage.gameObject.activeSelf)
        {
            completedImage.color = targetColor;
        }
    }
}
