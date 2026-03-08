using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the box is the Player
        if (other.CompareTag("Player"))
        {
            // Call the Singleton we set up in the Manager
            ObjectiveManager.Instance.hasSteppedInside = true;            
            // Optional: Destroy this trigger so it doesn't fire again
            Destroy(gameObject);
        }
    }
}