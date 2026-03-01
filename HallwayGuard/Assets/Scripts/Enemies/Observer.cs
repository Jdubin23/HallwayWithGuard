using UnityEngine;
using System.Collections;


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
            StartCoroutine(chaseTiming());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == Player)
        {
            PlayerInRange = false;
            // Player has left the observer's range
            Debug.Log("Player Lost!");
        }
    }

    IEnumerator chaseTiming ()
    {
        yield return new WaitForSeconds(2f);

        if (PlayerInRange)
        {
            Guard.Chase();
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerInRange) return;


        if (PlayerInRange)
        {
            Vector3 Origin = transform.position; // Ray starts from the guards position
            Vector3 direction = Player.position - Origin;
            float distance = direction.magnitude; // limits how far ray goes


            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, distance))
            {
                // If it hits the Guard (the parent), it will fail this 'if'
                if (hit.collider.transform == Player)
                {
                    Debug.Log("Prepare for dash!");
                    chaseTiming();
                }
                else
                {

                }
            }
        }
    }
}
