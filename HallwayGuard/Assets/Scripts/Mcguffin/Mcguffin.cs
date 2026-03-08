using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management


public class Mcguffin : MonoBehaviour, Interactable
{
    public void Interact()
    {
        // Toggle the state

        FindFirstObjectByType<GameEndManager>().WinGame();        // Perform the action

    }
}
