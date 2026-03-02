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


    [Header("State Settings")] // Bools for what state the guard is currently in. Might make these separate classes inheriting the behavior depending on the how the script develops
    public bool IsChasing = false; // Only enables once the player is detected, disables patrol.
    public bool IsPatrolling = true; // Default State, only deactivates once chasing is enabled.
    public bool LostPlayer = false; // Activates once the player is lost, disables chase.
    public bool IsCautious = false; // Activates once the player is lost, disables chase, activates caution.
    

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
        if (IsPatrolling)
            {
                Patrol();
            }
        if (IsChasing)
            {
                transform.position += chaseDirection * chaseSpeed * Time.deltaTime;
                
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
        CancelInvoke(nameof(Caution));
        Agent.speed = 6f; // Use Agent.speed to actually change NavMesh movement
        IsPatrolling = false;
        
        IsChasing = true; // You need to set this to true so Update() knows to follow the player

        lastLocation = Player.position;

        chaseDirection = (lastLocation - transform.position).normalized;

        transform.forward = chaseDirection;
        
        Agent.enabled = false;
        
        StartCoroutine(ChaseDelay());
        
    }

    IEnumerator ChaseDelay()
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
        IsChasing = false;

        Agent.enabled = true;
        IsPatrolling = true;

        Agent.SetDestination(Waypoints2[m_CurrentWaypointIndex].position);
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
            Stun();
        }
        
        if (collision.transform == Player)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
        }
    }

    

}
