using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UniversalMenuNav : MonoBehaviour
{
    public GameObject firstButton;
    private Vector2 lastMousePosition;

    void Update()
    {
        // 1. Detect if the mouse has actually moved
        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        if (Vector2.Distance(currentMousePos, lastMousePosition) > 0.1f)
        {
            // If the mouse moves, we should NOT have a 'Selected' object 
            // because the mouse uses 'Hover' instead.
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                // This stops the 'Permanent Hover' look when switching to mouse
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        lastMousePosition = currentMousePos;

        // 2. If the user hits a key or stick, snap back to the button
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame || Gamepad.current?.leftStick.ReadValue().magnitude > 0.1f)
            {
                EventSystem.current.SetSelectedGameObject(firstButton);
            }
        }
    }
}

public class CursorManager : MonoBehaviour
{
    private Vector2 lastMousePosition;

    void Start()
    {
        // Start by showing the cursor if you're on a PC
        Cursor.visible = true;
    }

    void Update()
    {
        // 1. Check if the mouse has moved
        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        if (Vector2.Distance(currentMousePos, lastMousePosition) > 1.0f)
        {
            ShowCursor();
        }
        lastMousePosition = currentMousePos;
        bool wasdPressed = Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed || 
                            Keyboard.current.sKey.isPressed || Keyboard.current.dKey.isPressed;

        // 2. Check if a controller or keyboard 'Navigation' key was pressed
        if ((Gamepad.current != null && Gamepad.current.leftStick.ReadValue().magnitude > 0.1f) || wasdPressed)        
        {
            HideCursor();
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            // Optional: You might want to show cursor on keyboard press, 
            // or keep it hidden if they are using WASD to navigate.
            HideCursor(); 
        }
    }

    void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // Prevents mouse from hovering invisible buttons
    }

    void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}