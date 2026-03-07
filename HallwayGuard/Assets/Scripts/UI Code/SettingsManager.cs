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
    public PlayerController playerScript; // Drag your Player object here
    public Volume globalVolume; // Drag your Global Volume here
    private ColorAdjustments colorAdjustments; // Or ColorGrading for Built-in

    void Start()
    {
    
    // The second parameter in GetFloat is the "Default Value" 
    // it uses if it can't find a saved setting.
    float savedVolume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
    float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);

    // Set the sliders to these values
    volumeSlider.value = savedVolume;
    sensitivitySlider.value = savedSens;

    // Apply them
    ApplyVolume(savedVolume);
    ApplySensitivity(savedSens);
    }
    void Awake()
    {
        if (globalVolume != null && globalVolume.profile != null)
    {
        if (globalVolume.profile.TryGet(out ColorAdjustments ca))
        {
            colorAdjustments = ca;
            Debug.Log("Brightness System Linked Successfully!");
        }
        else
        {
            Debug.LogError("Color Adjustments override is missing from the Volume Profile!");
        }
    }
    }

    public void ApplyVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void ApplySensitivity(float value)
    {
        if (playerScript != null)
        {
            // Update the variable in your PlayerController
            playerScript.Sensitivity = value; 
        }
        PlayerPrefs.SetFloat("MouseSensitivity", value);
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