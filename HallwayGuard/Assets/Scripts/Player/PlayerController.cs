using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    public float Speed = 5f;
    public Vector2 MovementInputVector;


    [Header("Camera Settings")]
    public float MouseSensitivity = 2f;
    public Vector2 lookInputVector;
    private float xRotation = 0f;  
    public Transform CameraTransform;             // Reference to the camera transform


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
    }

    private void Update()
    {
        ManageMovement();
        ManageLooking();

    }


    private void ManageMovement()
    {
        //change the player object's position based on the input values
        Vector3 posChange = transform.right * MovementInputVector.x + transform.forward * MovementInputVector.y;
        //change position of object based on new vectors we collected
        transform.position += posChange * Time.deltaTime * Speed; //5f is the speed of the player, can be changed to make the player move faster or slower
    }

    private void ManageLooking()
    {
        
         // Horizontal rotation (turns the player body)
        float mouseX = lookInputVector.x * MouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation (tilts the camera)
        float mouseY = lookInputVector.y * MouseSensitivity;
        xRotation -= mouseY;                      // Invert for natural FPS feel
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevents over-rotation
        CameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }



}
