using UnityEngine;
using System.Collections.Generic;
public class LeverManager : MonoBehaviour
{
    [SerializeField] private BatterySocket socket; // Battery socket for referencing ResetColor()
    [SerializeField] private List<Lever> correctOrder;
    private int currentStep = 0;

    [Header("Action Settings")]
    [SerializeField] private GameObject targetObject; // Door to disappear when you complete the puzzle

    [Header("Audio Sources")]
    public AudioSource NoPowerAudio;
    public AudioSource WrongLeverAudio;
    public AudioSource LeverAudio;
    public AudioSource LaserGoneAudio;


    public void LeverPulled(Lever pulledLever)
    {
        // STEP 1: Check for power
        if (socket == null || !socket.IsPowered)
        {
            Debug.Log("No Power! The lever won't budge."); //add error audio
            if (NoPowerAudio != null && !NoPowerAudio.isPlaying)
                {
                    NoPowerAudio.Play();
                }
            return; 
        }

        // STEP 2: Normal logic runs only if powered
        if (pulledLever == correctOrder[currentStep])
        {
            if (LeverAudio != null && !LeverAudio.isPlaying)
                {
                 LeverAudio.Play();   
                }
                
            pulledLever.SetState(true);
            currentStep++;

            if (currentStep >= correctOrder.Count)
            {
                if (LaserGoneAudio != null && !LaserGoneAudio.isPlaying)
                        {
                            LaserGoneAudio.Play();
                        }
                if (targetObject != null) targetObject.SetActive(false); // Make the door disappear if there is one assigned
                ObjectiveManager.Instance.hasFlippedLevers = true;
            }
        }
        else
        {
            if (WrongLeverAudio != null && !WrongLeverAudio.isPlaying)
                {
                    WrongLeverAudio.Play();
                }
            ResetLevers();
        }
    }

    private void ResetLevers()
    {
        currentStep = 0;
        foreach (Lever lever in correctOrder)
        {
            lever.SetState(false);
            //error audio added here for wrong order
        }
    }
}