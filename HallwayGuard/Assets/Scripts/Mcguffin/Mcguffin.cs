using UnityEngine;

public class Mcguffin : MonoBehaviour, Interactable
{
    private bool _alreadyInteracted = false;

    public void Interact()
    {
        if (_alreadyInteracted) return;

        // Use a more reliable way to find the manager
        GameEndManager gem = Object.FindFirstObjectByType<GameEndManager>();

        if (gem != null)
        {
            _alreadyInteracted = true;
            gem.WinGame(); // This calls the public method that starts the coroutine
            
        }
        else
        {
            
        }
    }
}