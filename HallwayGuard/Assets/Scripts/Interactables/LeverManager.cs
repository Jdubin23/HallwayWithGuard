using UnityEngine;
using System.Collections.Generic;
public class LeverManager : MonoBehaviour
{
    [SerializeField] private BatterySocket socket; // Battery socket for referencing ResetColor()
    [SerializeField] private List<Lever> correctOrder;
    private int currentStep = 0;

    [Header("Action Settings")]
    [SerializeField] private GameObject targetObject; // Door to disappear when you complete the puzzle
    public void LeverPulled(Lever pulledLever)
    {
        // STEP 1: Check for power
        if (socket == null || !socket.IsPowered)
        {
            Debug.Log("No Power! The lever won't budge."); //add error audio
            return; 
        }

        // STEP 2: Normal logic runs only if powered
        if (pulledLever == correctOrder[currentStep])
        {
            pulledLever.SetState(true);
            currentStep++;

            if (currentStep >= correctOrder.Count)
            {
                Debug.Log("Puzzle Solved!"); //add some success audio
                if (targetObject != null) targetObject.SetActive(false); // Make the door disappear if there is one assigned
            }
        }
        else
        {
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