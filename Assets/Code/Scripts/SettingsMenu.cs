using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource bgmSource; // Reference to background music AudioSource
    public AudioSource[] initialSfxSources; // Initial SFX AudioSources added via Inspector (optional)

    private List<AudioSource> sfxSources = new List<AudioSource>(); // A list for all SFX AudioSources

    [Header("Sliders")]
    public Slider musicVolumeSlider; // Reference to the music volume slider
    public Slider sfxVolumeSlider; // Reference to the SFX volume slider

    private float musicVolume = 1f; // Default background music volume
    private float sfxVolume = 1f; // Default SFX volume

    private void Awake()
    {
        // Load saved volume levels
        LoadVolumeSettings();
    }

    private void Start()
    {
        // Add the initial SFX sources assigned in the Inspector (optional)
        if (initialSfxSources.Length > 0)
        {
            sfxSources.AddRange(initialSfxSources);
        }

        // Get the current scene name
        string currentScene = SceneManager.GetActiveScene().name;

        // Check if we're not in the "Menu" or "Tutorial" scenes before fetching audio sources from the canvases
        if (currentScene != "Menu" && currentScene != "Tutorial")
        {
            // Dynamically find AudioSources from Victory Canvas and Game Over Canvas
            AudioSource victoryCanvasAudioSource = GameObject.Find("Victory Canvas")?.GetComponent<AudioSource>();
            AudioSource gameOverCanvasAudioSource = GameObject.Find("Game Over Canvas")?.GetComponent<AudioSource>();

            if (victoryCanvasAudioSource != null)
                sfxSources.Add(victoryCanvasAudioSource);

            if (gameOverCanvasAudioSource != null)
                sfxSources.Add(gameOverCanvasAudioSource);
        }

        // Load the saved volume settings and initialize sliders
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }

        // Add listeners to update volumes dynamically
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // Set background music volume
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        bgmSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    // Set SFX volume
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        foreach (var sfxSource in sfxSources)
        {
            sfxSource.volume = sfxVolume;
        }
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    // Load the saved volume settings from PlayerPrefs
    private void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            bgmSource.volume = musicVolume;
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        }
    }
}
