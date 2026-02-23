using UnityEngine;

public class Observer : MonoBehaviour
{
    public Transform Player; // Reference to the player's transform
    public SmallGuard Guard; // Reference to the SmallGuard script to communicate player detection status
    public bool PlayerInRange;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == Player)
        {
            PlayerInRange = true; // Player has entered the observer's range
            Debug.Log("Player Detected!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == Player)
        {
            PlayerInRange = false; // Player has left the observer's range
            Debug.Log("Player Lost!");
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerInRange)
        {
            Vector3 direction = Player.position - transform.position + Vector3.up;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform == Player)
                {
                    PlayerInRange = true;
                    Guard.Chase(); // Call the Chase method in the SmallGuard script to start chasing the player
                    Debug.Log("Player Detected!");
                }
                else
                {
                    PlayerInRange = false;
                    Debug.Log("Player Lost!");
                }
            }
        }
    }
}
