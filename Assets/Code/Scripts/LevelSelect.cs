using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private bool unlocked; // Default value is false;
    public Image unlockImage;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
    }

    private void Update()
    {
        UpdateLevelImage();
        UpdateLevelStatus();
    }

    public void UpdateLevelImage()
    {
        if (!unlocked)
        {
            unlockImage.gameObject.SetActive(true);
        }
        else
        {
            unlockImage.gameObject.SetActive(false);
        }
    }

    private void UpdateLevelStatus()
    {
        // If the current level is 5, the previous should be 4
        /*int previousLevelNum = int.Parse(gameObject.name) - 1;
        if (PlayerPrefs.GetInt("Lv" + previousLevelNum) > 0)
        {
            unlocked = true;
        }*/
    }

    public void PressSelection(string _LevelName) // Move to the corrisponding level when you click the button
    {
        if (unlocked == true)
        {
            SceneManager.LoadScene(_LevelName);
        }
    }
}
