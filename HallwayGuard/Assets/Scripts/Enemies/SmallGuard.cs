using UnityEngine;
using UnityEngine.AI; // For NavMeshAgent
using System.Collections;
using System.Collections.Generic;


public class SmallGuard : MonoBehaviour
{
    
    public Transform Player; // Reference to the player's transform
    
    [Header("Enemy Settings")]
    public float Speed = 3f;
    public NavMeshAgent Agent; // Reference to the NavMeshAgent component
    public Transform[] Waypoints; // Array of patrol points for the guard to move between
    int m_CurrentWaypointIndex; 
    public Collider VisionCollider; // Collider used to detect the player within the guard's vision range
    public bool PlayerDetected = false;

    public float ChaseDuration;
    public float CautionDuration;
    

    [Header("State Settings")] // Bools for what state the guard is currently in. Might make these separate classes inheriting the behavior depending on the how the script develops
    public bool IsChasing = false; // Only enables once the player is detected, disables patrol.
    public bool IsPatrolling = true; // Default State, only deactivates once chasing is enabled.
    public bool LostPlayer = false; // Activates once the player is lost, disables chase.
    public bool IsCautious = false; // Activates once the player is lost, disables chase, activates caution.

    private void Awake()
    {
        Player = GameObject.Find("Player").transform; // Finds the player in the scene and assigns it to the Player variable
        // will be useful for chasing the player
    }
    
    private void Patrol()
    {
        
        if(Agent.remainingDistance < Agent.stoppingDistance)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % Waypoints.Length; // Moves guard to the following waypoints
            Agent.SetDestination(Waypoints[m_CurrentWaypointIndex].position);
        }

        
        
    }
    private void Chase()
    {
        IsChasing = true; // sets state as true
        IsPatrolling = false;

        Speed = 4f; // Increase speed when chasing
        
        Agent.SetDestination(Player.position); // Chase the player
    }

    private void LosingPlayer()
    {
        IsChasing = false; // sets state as false
        LostPlayer = true; // sets state as true

        Speed = 2f; // Decrease speed when losing the player
        
        Agent.SetDestination(Player.position); // Move towards the last known player position

    }

    private void capturePlayer()
    {
        
    }
    private void Caution()
    {
        /* Things to include */
        /* 1. Caution Movement (Moves towards last known player position, doesn't collide with walls) */
    }

    void Start()
    {
        Agent.SetDestination(Waypoints[0].position); // Start patrolling towards the first waypoint
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPatrolling)
        {
            Patrol(); // Call the Patrol method every frame to handle patrolling behavior
        }
        else if (IsChasing)
        {
            Chase(); // Call the Chase method every frame to handle chasing behavior
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == Player)
        {
            IsChasing = true; // Player detected, start chasing
        }
    }

    
}
