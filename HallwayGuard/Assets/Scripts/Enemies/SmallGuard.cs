using UnityEngine;
using UnityEngine.AI; // For NavMeshAgent
using System.Collections;
using System.Collections.Generic;


public class SmallGuard : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float Speed = 3f;
    public NavMeshAgent Agent; // Reference to the NavMeshAgent component
    public Transform[] Waypoints; // Array of patrol points for the guard to move between

    [Header("State Settings")] // Bools for what state the guard is currently in. Might make these separate classes inheriting the behavior depending on the how the script develops
    public bool IsChasing = false; // Only enables once the player is detected, disables patrol.
    public bool IsPatrolling = true; // Default State, only deactivates once chasing is enabled.
    public bool LostPlayer = false; // Activates once the player is lost, disables chase.
    public bool IsCautious = false; // Activates once the player is lost, disables chase, activates caution.

    /*  State Flow Chart
    Patrol/Normal -> Chase -> Caution -> LostPlayer -> Patrol/Normal
    */


    
    
    
    
    
    /*
    possible states:
    Normal/ patrol - standard search
    chase - finds player with hitbox
    LostPlayer - happens 
    caution
    */


    /* Detect player method
    Detection rate is going to be made with invisible box with collider attached to enemy guard
    - once in range of the hitbox, bar is gonna fill, enemy chases player when full
    - if bar is full
    - if the enemy is looking at a wall of another obstacle, it cannot see anything beyond it.
    {
        patrol state is disabled, chase state is enabled, enemy chases player
    }
    */

    private void Patrol()
    {
        /* Things to include */
        /* 1. Patrol Movement (Moves in random directions based on waypoints, doesn't collide with walls */ 
    }
    private void Chase()
    {
        /* Things to include */
        /* 1. Chase Movement (Moves towards player, doesn't collide with walls) 
           2. Once player leaves range of detection, chase will last for a limited time and eventually 
           transition to LostPlayer */

    }

    private void Caution()
    {
        /* Things to include */
        /* 1. Caution Movement (Moves towards last known player position, doesn't collide with walls) */
    }

    


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
