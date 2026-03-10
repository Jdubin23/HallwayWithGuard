using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class Lever : MonoBehaviour, Interactable
{
    [Header("Lever Settings")]
    public int LeverID; // Used to show order of levers in the inspector
    [SerializeField] private bool isOn = false;
    [SerializeField] private Transform leverHandle; // Reference to the part that moves
    [SerializeField] private float LeverOn = 120f;
    [SerializeField] private float LeverOff = 0f;
    [SerializeField] private float speed = 4f; // Added for control over the speed of the lever movement
    [SerializeField] private bool isLocked = false; // This checks if its interactable or not

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
     
     if (isLocked) return;
     
        // Toggle the state
        isOn = !isOn;
        StopAllCoroutines(); 
        StartCoroutine(MoveLever());

        // Perform the action
        if (isOn)
        {
            Debug.Log("Lever Pulled: ON");
            
            // Tell the manager this lever was pulled
            Object.FindFirstObjectByType<LeverManager>().LeverPulled(this);
        }
    }
    public void SetLock(bool lockedState)
    {
        isLocked = lockedState;
    }

private IEnumerator MoveLever()
    {
        float targetAngle = isOn ? LeverOn : LeverOff;
        Quaternion targetRot = Quaternion.Euler(targetAngle, 0f, 0f);

        // While the handle isn't basically at the target yet...
        while (Quaternion.Angle(leverHandle.localRotation, targetRot) > 0.01f)
        {
            // Rotate towards the target angle at a constant speed
            leverHandle.localRotation = Quaternion.RotateTowards(
                leverHandle.localRotation, 
                targetRot, 
                speed * 100f * Time.deltaTime
            );
            
            yield return null; // Wait for next frame
        }
        
        leverHandle.localRotation = targetRot; // Snap to perfect finish
    }
    
    public void PulseLever()
        {
            StopAllCoroutines();
            StartCoroutine(PulseRoutine());
        }

        private IEnumerator PulseRoutine()
        {
            // 1. Move Down
            isOn = true; 
            yield return StartCoroutine(MoveLever());
            
            // 2. Short pause at the bottom for impact
            yield return new WaitForSeconds(0.2f); 
            
            // 3. Move back Up
            isOn = false;
            yield return StartCoroutine(MoveLever());
        }

    public void SetState(bool state)
    {
        isOn = state;
        if (isOn) isLocked = true; // Lock the lever in place if it's turned on
        else isLocked = false;
        
        Color targetColor = isOn ? activeColor : inactiveColor;
        foreach (Renderer rend in targetRenderers)
        {
            if (rend != null)
            {
                // .material creates a unique instance so they don't all change at once
                rend.material.color = targetColor;
            }
        }
        StopAllCoroutines(); 
        StartCoroutine(MoveLever());
    }
}