using UnityEngine;

public class Lever : MonoBehaviour, Interactable
{
    [Header("Animation Settings")]
    [SerializeField] private bool isOn = false;
    [SerializeField] private Transform leverHandle; // Reference to the part that moves
    [SerializeField] private float LeverOn = 45f;
    [SerializeField] private float LeverOff = -45f;

    [Header("Action Settings")]
    [SerializeField] private GameObject targetObject; // Drag the object you want to disappear here
    [SerializeField] private bool hideWhenOn = true;  // Should it disappear when lever is pulled?

    public void Interact()
    {
        // Toggle the state
        isOn = !isOn;

        // Perform the action
        if (isOn)
        {
            Debug.Log("Lever Pulled: ON");
            leverHandle.localRotation = Quaternion.Euler(LeverOn, 0, 0);
            // Add code here to open a door, turn on a light, etc.
            if(targetObject != null)
        {
                targetObject.SetActive(!(isOn && hideWhenOn)); //make door disappear when lever is on.
            }
        }
        else
        {
            Debug.Log("Lever Pulled: OFF");
        }
    }
}