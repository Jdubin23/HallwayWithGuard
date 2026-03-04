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
    public Transform[] Waypoints2; // Array of patrol points for the guard to move between
    int m_CurrentWaypointIndex;
    public Collider VisionCollider; // Collider used to detect the player within the guard's vision range
    public bool PlayerDetected = false;

    [Header("Chase Settings")]
    private float ChaseDuration = 2;
    private float CautionDuration;
    public float chaseDelay;
    private Vector3 lastLocation; //last location player was seen in
    private Vector3 chaseDirection;
    public float chaseSpeed;

    [Header("Stun Settings")]
    public float stunDuration = 4f;
    


    [Header("State Settings")] // Bools for what state the guard is currently in. Might make these separate classes inheriting the behavior depending on the how the script develops
    public bool IsChasing = false; // Only enables once the player is detected, disables patrol.
    public bool IsPatrolling = true; // Default State, only deactivates once chasing is enabled.
    public bool LostPlayer = false; // Activates once the player is lost, disables chase.
    public bool IsCautious = false; // Activates once the player is lost, disables chase, activates caution.
    public bool IsStunned = false;

    #region Awake, Start, Update
    private void Awake()
    {
        Player = GameObject.Find("Player").transform; // Finds the player in the scene and assigns it to the Player variable
        // will be useful for chasing the player
    }

    void Start()
    {
        Agent.SetDestination(Waypoints2[0].position); // Start patrolling towards the first waypoint
    }

    // Update is called once per frame
    void Update()
    {
        if (IsStunned)
        {
            return; // prevents any other behavior while stunned
        }
        if (IsPatrolling)
        {
            Patrol();
        }
        if (IsChasing)
        {
            float moveDistance = chaseSpeed * Time.deltaTime;

            if(!Physics.Raycast(transform.position, chaseDirection, moveDistance))
            {
                transform.position += chaseDirection * moveDistance;
            }
            else
            {
                Stun();
            }
            
            
            //transform.position += chaseDirection * chaseSpeed * Time.deltaTime;
                
            //Invoke(nameof(Chase), chaseDelay); // Chase the player
        }
    }

    
    #endregion

    #region State Behaviors
    private void Patrol()
    {
        if (!Agent.enabled) return;
        
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
        if (IsChasing) return; // prevents enemy from homing into the player
        
        CancelInvoke(nameof(Caution));
        Agent.speed = 6f; // Use Agent.speed to actually change NavMesh movement
        IsPatrolling = false;
        IsStunned = false;

        lastLocation = Player.position;
        Vector3 Grounded = new Vector3(lastLocation.x, transform.position.y, lastLocation.z);
        // If only using last location, guard lifts off the ground.

        chaseDirection = (Grounded - transform.position).normalized;

        transform.forward = chaseDirection;
        
        Agent.enabled = false;
        
        StartCoroutine(ChaseDelay());

        
        
    }

    IEnumerator ChaseDelay() //causes guard to wait a bit before chasign the player
    {
        yield return new WaitForSeconds(chaseDelay);

        IsChasing = true;
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
        Agent.enabled = true;
        Agent.speed = Speed; // Reset speed to default
    }

    public void Stun()
    {
        // Behavior after hitting wall
        IsStunned = true;
        IsChasing = false; //chase behavior stops completely
        IsPatrolling = false;

        Agent.enabled = true; // navMesh still not enabled

        Invoke(nameof(Recover), stunDuration);
    }

    public void Recover()
    {
        Agent.enabled = true; //navMesh is available again, allowing patrolling
        Agent.speed = Speed; // resets speed back to normal
        IsPatrolling = true; // enables patrolling

        IsStunned = false;

        Agent.SetDestination(Waypoints2[m_CurrentWaypointIndex].position); //reenables waypoints
        
    }
    #endregion

    //when colliding with player, End game or trigger capture state
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object we bumped into is the player
        if (!IsChasing)
        {
            return;
        }
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Agent.enabled = true; //transform.position ignores colliders, so we enable the agent 
            Stun();
        }
        
        if (collision.transform == Player)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
        }
    }

    

}
