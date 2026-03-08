using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // For NavMeshAgent
using UnityEngine.SceneManagement; 

public class BigGuard : MonoBehaviour
{

    public Transform Player; // Reference to the player's transform
    public Observer Observer; // Reference to the Observer script to communicate player detection status

    [Header("Enemy Settings")]
    public float Speed = 3f;
    public NavMeshAgent Agent; // Reference to the NavMeshAgent component
    public Transform[] Waypoints2; // Array of patrol points for the guard to move between
    int m_CurrentWaypointIndex;
    public Collider VisionCollider; // Collider used to detect the player within the guard's vision range
    public bool PlayerDetected = false;
    private float ChaseDuration = 2;


    [Header("State Settings")] // Bools for what state the guard is currently in. Might make these separate classes inheriting the behavior depending on the how the script develops
    public bool IsChasing = false; // Only enables once the player is detected, disables patrol.
    public bool IsPatrolling = true; // Default State, only deactivates once chasing is enabled.
    public bool LostPlayer = false; // Activates once the player is lost, disables chase.
    public bool IsCautious = false; // Activates once the player is lost, disables chase, activates caution.

    [Header("Audio Sources")]
    public AudioSource chaseAudio;
    public AudioSource SlowStompAudio;



    private void Awake()
    {
        Player = GameObject.Find("Player").transform; // Finds the player in the scene and assigns it to the Player variable
        // will be useful for chasing the player
    }

    void Start()
    {
        Agent.SetDestination(Waypoints2[0].position); // Start patrolling towards the first waypoint
        if(chaseAudio == null) chaseAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SlowStompAudio != null && !SlowStompAudio.isPlaying)
                {
                    SlowStompAudio.Play();
                }
        if (IsPatrolling)
            {
                Patrol();
            }
        if (IsChasing)
            {
                Agent.SetDestination(Player.position); // Chase the player
            }
    }

    #region State Behaviors
    private void Patrol()
    {
        if (chaseAudio != null && chaseAudio.isPlaying)
        {
            chaseAudio.Stop();
        }

            
        if (IsPatrolling)
        {
            if (Agent.remainingDistance < Agent.stoppingDistance)
            {
                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % Waypoints2.Length; // Moves guard to the following waypoints
                Agent.SetDestination(Waypoints2[m_CurrentWaypointIndex].position);
            }
        }

    }
    public void Chase()
    {
        CancelInvoke(nameof(Caution));
        IsPatrolling = false;

        Agent.speed = 3.5f; // Use Agent.speed to actually change NavMesh movement

        // Remove the 'if (Observer.PlayerInRange)' line
        IsChasing = true; // You need to set this to true so Update() knows to follow the player
        if (chaseAudio != null && !chaseAudio.isPlaying)
        {
            chaseAudio.Play();
        }

    }
    public void StartChaseTimer()
    {
        // Only start the countdown if we are currently chasing
        if (IsChasing)
        {
            Invoke(nameof(Caution), ChaseDuration); // ChaseDuration is your 2 seconds
        }
    }
    public void Caution()
    {

        IsChasing = false; // cancels chase
        IsPatrolling = true; // makes patrolling true
        Agent.speed = Speed; // Reset speed to default
        
    }
    #endregion

    //when colliding with player, End game or trigger capture state
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object we bumped into is the player
        if (collision.transform == Player)
        {
                FindFirstObjectByType<GameEndManager>().ActivateLoseUI();        // Perform the action

            
        }
    }
 }
