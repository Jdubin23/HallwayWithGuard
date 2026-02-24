using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management


public class Mcguffin : MonoBehaviour, Interactable
{
    public void Interact()
    {
        // Toggle the state
       
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Perform the action
       
    }
}
