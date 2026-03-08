using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    public float Speed = 5f;
    public Vector2 MovementInputVector;
    public Rigidbody rb;
    public AudioSource MovementAudio;
    public float footstepInterval = 0.5f; // Time between steps
    private float footstepTimer;



    [Header("Camera Settings")]
    public float Sensitivity = 2f;
    public Vector2 lookInputVector;
    private float xRotation = 0f;
    public Transform CameraTransform;             // Reference to the camera transform
    [SerializeField] private float controllerBaseWeight = 125f; // Multiplier to equal controller movement to mouse movement
    [SerializeField] private float mouseDamping = 0.05f; //limiter to reduce sensitivity back to normal amount
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer;
    private Camera cam;

    [Header("Jump Settings")]
    public float JumpHeight = 1f;                // Desired jump height
    public bool IsGrounded;                      // Tracks if the player is touching the ground



    #region Input System Callbacks

    private void OnMove(InputValue inputValue)
    {
        //grab values from unity input system (for both mediums) and set it to this variable
        MovementInputVector = inputValue.Get<Vector2>();

        //Debug.Log(inputValue.Get<Vector2>()); to show a button was pressed and recieved for debugging purposes, can be removed later
    }

    private void OnLook(InputValue inputValue)
    {
        //grab values from unity input system (for both mediums)
        lookInputVector = inputValue.Get<Vector2>();
    }


    public void OnJump(InputValue Value)
    {
        // Only jump when the button is first pressed AND the player is grounded
        if (Value.isPressed && IsGrounded)
        {
            // Calculates the upward velocity needed to reach the desired jump height
            float jumpVelocity = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);

            // Apply to the Rigidbody
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;
        // Fire a ray from the center of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            Debug.Log("Raycast hit: " + hit.collider.name); // Debug log to confirm we hit something
            // Check if the object has an IInteractable interface
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
            if (interactable != null)
            {
                Debug.Log("Interactable object hit: " + hit.collider.name); // Debug log to confirm we hit an interactable object
                interactable.Interact();
            }
        }
    }

    #endregion


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // The cursor is automatically invisible when locked
        Cursor.visible = false;

    }

    void Start()
    {
        CameraTransform.localRotation = Quaternion.Euler(90f, 90f, 90f); // Set initial rotation to Face forward
        cam = Camera.main;
    }

    private void Update()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

    }
    private void LateUpdate()
    {
        ManageCameraRotation();
    }
    private void FixedUpdate()
    {
        ManageMovement();
       
    }


    private void ManageMovement()
    {
        //change the player object's position based on the input values
        Vector3 posChange = transform.right * MovementInputVector.x + transform.forward * MovementInputVector.y;
        //change position of object based on new vectors we collected
        rb.linearVelocity = new Vector3(posChange.x * Speed, rb.linearVelocity.y, posChange.z * Speed);
        if (IsGrounded && posChange.magnitude > 0.1f)
    {
        footstepTimer -= Time.fixedDeltaTime;

        if (footstepTimer <= 0)
        {
            // Play the sound
            if (MovementAudio != null)
            {
                // Optional: Vary the pitch slightly so it sounds more natural
                MovementAudio.pitch = Random.Range(0.9f, 1.1f);
                MovementAudio.Play();
            }
            
            // Reset timer
            footstepTimer = footstepInterval;
        }
    }
    else
    {
        // Reset timer when standing still so the first step happens instantly next time
        footstepTimer = 0; 
    }
    }

    private void ManageCameraRotation()
    {
        // 1. Identify the device
        bool isGamepad = Gamepad.current != null &&
                         lookInputVector == Gamepad.current.rightStick.ReadValue();

        float finalX, finalY;

        if (isGamepad)
        {
            // For Controller: Use the Base Weight * User Sensitivity * DeltaTime
            // This scales the 0-1 stick input into a usable rotation speed
            finalX = lookInputVector.x * (Sensitivity * controllerBaseWeight) * Time.deltaTime;
            finalY = lookInputVector.y * (Sensitivity * controllerBaseWeight) * Time.deltaTime;
        }
        else
        {
            // For Mouse: Just the Raw Delta * User Sensitivity
            finalX = lookInputVector.x * (Sensitivity * mouseDamping);
            finalY = lookInputVector.y * (Sensitivity * mouseDamping);
        }

        // 2. Apply Rotations
        xRotation -= finalY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        CameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, finalX, 0));
    }



}
public interface Interactable
{
    void Interact();
}