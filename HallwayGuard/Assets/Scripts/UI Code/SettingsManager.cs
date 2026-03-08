using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering; // Required for Volumes
using UnityEngine.Rendering.Universal; // Use this if on URP

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public GameObject firstSettingsButton;

    [Header("Default Values")]
    [SerializeField] private float defaultVolume = 0.5f;
    [SerializeField] private float defaultSensitivity = 2.0f;

    [Header("References")]
    private PlayerController playerScript; // Drag your Player object here
    private bool _blockSave = true; // Forces Audio to save

    void Start()
    {

    _blockSave = true;

    float savedVolume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
    float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);

        // Update UI based on saved values 
        if (volumeSlider != null) volumeSlider.value = savedVolume;
        if (sensitivitySlider != null) sensitivitySlider.value = savedSens;

        // find the Player automatically if we are in the Game scene
        FindPlayer();

        // Initial Application
        ApplyVolume(savedVolume);
        ApplySensitivity(savedSens);
    
    _blockSave = false;
    }
    
    private void FindPlayer()
    {
        if (playerScript == null)
        {
            playerScript = Object.FindFirstObjectByType<PlayerController>();
        }
    }

    public void ApplyVolume(float value)
    {
        AudioListener.volume = value;
        if (!_blockSave)
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
            PlayerPrefs.Save(); 
        }
    }

    public void ApplySensitivity(float value)
    {
        if (playerScript != null)
        {
            // Update the variable in your PlayerController
            playerScript.Sensitivity = value; 
        }
        if (!_blockSave)
        {
            PlayerPrefs.SetFloat("MouseSensitivity", value);
            PlayerPrefs.Save();
        }
    }

    

    public void ResetSettings()
    {
        volumeSlider.value = defaultVolume;
        sensitivitySlider.value = defaultSensitivity;

        ApplyVolume(defaultVolume);
        ApplySensitivity(defaultSensitivity);

        PlayerPrefs.Save();
    }
}