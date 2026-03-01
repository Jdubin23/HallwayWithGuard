using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // For NavMeshAgent
using UnityEngine.SceneManagement;


public class SmallGuard : MonoBehaviour
{
    
    public Transform Player; // Reference to the player's transform
    public Observer Observer; // Reference to the Observer script to communicate player detection status
    
    [Header("Enemy Settings")]
    public float Speed = 3f;
    public NavMeshAgent Agent; // Reference to the NavMeshAgent component
    public Transform[] Waypoints; // Array of patrol points for the guard to move between
    int m_CurrentWaypointIndex; 
    public Collider VisionCollider; // Collider used to detect the player within the guard's vision range
    public bool PlayerDetected = false;
    public float ChaseDuration;
    public float CautionDuration;

    [Header("Dash Settings")]
    // public float dashSpeed;
    public float dashDuration;
    private Vector3 dashDirection;
    public float stunDuration;
    private float dashTimer;
    private Vector3 savedPosition; //last saved player position
    public bool isDashing;
    

    public LayerMask Wall;
    
    

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
    
    
    #region State Behaviors
    private void Patrol()
    {
        if (IsPatrolling)
        {
            if(Agent.remainingDistance < Agent.stoppingDistance)
            {
                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % Waypoints.Length; // Moves guard to the following waypoints
                Agent.SetDestination(Waypoints[m_CurrentWaypointIndex].position);
            } 
        }
        
    }
    public void Chase()
    {
        CancelInvoke(nameof(Caution));
        IsPatrolling = false;
        IsChasing = true;
        Agent.speed = 10f; // Use Agent.speed to actually change NavMesh movement


        // Chase behavior is a single, fast dash to the player.
        Agent.enabled = false; // disable navmesh.

        
        
        dashTimer = dashDuration;
        savedPosition = Player.position;
        
        dashDirection = (savedPosition - transform.position).normalized;
        transform.forward = dashDirection;
        
        
        //transform.position += dashDirection * Agent.speed * Time.deltaTime;
        //dashTimer -= Time.deltaTime;
        
        
        /*
        if (dashTimer <= 0f)
        {
            Stun();
        }
        */

        // Remove the 'if (Observer.PlayerInRange)' line
        isDashing = true; // You need to set this to true so Update() knows to follow the player
        
        
    }
    void Dash()
    {
        transform.position += dashDirection * 10f * Time.deltaTime;
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

    public void Stun()
    {
        Debug.Log("Brother is Stunned! Get out of there!");
        IsChasing = false;
        IsPatrolling = false; // both states are false for a bit.
        
        Invoke(nameof(Patrol), stunDuration);
        // being stunned means 
    }
    #endregion
    

    void Start()
    {
        Agent.SetDestination(Waypoints[0].position); // Start patrolling towards the first waypoint
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPatrolling)
        {
            Patrol();
        }
        if (IsChasing)
        {
            Chase();
        }
        if (isDashing)
        {
            Dash();
        }
        

    }
    //when colliding with player, End game or trigger capture state
    void OnCollisionEnter(Collision collision)
    {
        if (IsChasing)
        {
            /*
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                Stun();
            }
            */


            if (collision.transform == Player)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            } 
        }
        
        // Check if the object we bumped into is the player
        
    }

    

}
