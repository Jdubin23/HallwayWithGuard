using UnityEngine;
using System.Collections.Generic;

public class Lever : MonoBehaviour, Interactable
{
    [Header("Lever Settings")]
    public int LeverID; // Used to show order of levers in the inspector
    [SerializeField] private bool isOn = false;
    [SerializeField] private Transform leverHandle; // Reference to the part that moves
    [SerializeField] private float LeverOn = 120f;
    [SerializeField] private float LeverOff = 0f;

    [Header("Light Settings")]
    [SerializeField] private List<Renderer> targetRenderers; 
    [SerializeField] private Color activeColor = Color.limeGreen;
    [SerializeField] private Color inactiveColor = Color.coral;

public void Start()
    {
        foreach (Renderer rend in targetRenderers)
        if (rend != null)
            {
                // .material creates a unique instance so they don't all change at once
                rend.material.color = inactiveColor;
            }
    }
    public void Interact()
    {
        // Toggle the state
        isOn = !isOn;

        // Perform the action
        if (isOn)
        {
            Debug.Log("Lever Pulled: ON");
            // Tell the manager this lever was pulled
            Object.FindFirstObjectByType<LeverManager>().LeverPulled(this);
        }
    }

    public void SetState(bool state)
    {
        isOn = state;
        float targetAngle = isOn ? LeverOn : LeverOff;
        leverHandle.localRotation = Quaternion.Euler(targetAngle, 0f, 0f);
        Color targetColor = isOn ? activeColor : inactiveColor;
        foreach (Renderer rend in targetRenderers)
        {
            if (rend != null)
            {
                // .material creates a unique instance so they don't all change at once
                rend.material.color = targetColor;
            }
        }
    }
}