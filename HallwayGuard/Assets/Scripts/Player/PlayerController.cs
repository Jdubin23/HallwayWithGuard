using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    public float Speed = 5f;
    public Vector2 MovementInputVector;
    public Rigidbody rb;


    [Header("Camera Settings")]
    public float MouseSensitivity = 2f;
    public Vector2 lookInputVector;
    private float xRotation = 0f;  
    public Transform CameraTransform;             // Reference to the camera transform

    [SerializeField] private float interactRange = 5f;
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


    public void OnJump(InputValue Value ) 
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
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
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
        // Handle horizontal body rotation here so physics doesn't fight it
        float mouseX = lookInputVector.x * MouseSensitivity;
        if (Mathf.Abs(mouseX) > 0.01f)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, mouseX, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }


    }
    private void LateUpdate()
    {
        ManageVerticalLooking();

    }
    private void FixedUpdate()
    {
        ManageMovement();
        
    }


    private void ManageMovement()
    {
        //change the player object's position based on the input values
        Vector3 posChange = (transform.right * MovementInputVector.x) + (transform.forward * MovementInputVector.y);
        //change position of object based on new vectors we collected
        //transform.position += posChange * Time.deltaTime * Speed; //5f is the speed of the player, can be changed to make the player move faster or slower
     rb.linearVelocity = new Vector3(posChange.x * Speed, rb.linearVelocity.y, posChange.z * Speed );
    }

    private void ManageVerticalLooking()
    {
        
         // Get Player Input
        float mouseY = lookInputVector.y * MouseSensitivity;

        xRotation -= mouseY;                      
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
        
        CameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }



}
public interface IInteractable
{
    void Interact();
}