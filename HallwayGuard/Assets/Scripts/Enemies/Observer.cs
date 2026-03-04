using UnityEngine;

public class Observer : MonoBehaviour
{
    public Transform Player; 
    public SmallGuard Guard; 
    public bool PlayerInRange;

    void OnTriggerEnter(Collider other)
    {
        // Use CompareTag for better performance and reliability
        if (other.CompareTag("Player"))
        {
            PlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;
            // Guard.StartChaseTimer(); // Optional: used for the 2-second caution window
        }
    }

    void Update()
    {
        if (!PlayerInRange || Player == null) return;

        // Origin point: move it slightly up (e.g., eye level) so it doesn't hit the floor
        Vector3 origin = transform.position + Vector3.up * 0.5f; 
        Vector3 targetPos = Player.position + Vector3.up * 0.5f;
        Vector3 direction = targetPos - origin;
        float distance = direction.magnitude;

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        // Use a LayerMask to ignore the Guard itself
        // Ensure the Guard is on the "Enemy" layer in the Inspector!
        int layerMask = ~LayerMask.GetMask("Enemy"); 

        if (Physics.Raycast(ray, out hit, distance, layerMask))
        {
            if (hit.collider.transform == Player)
            {
                // IMPORTANT: This calls the charge behavior
                Guard.Chase();
            }
        }
        
        // Debug line to see the vision in the Scene view (Green = seeing player)
        Debug.DrawRay(origin, direction, Color.green);
    }
}