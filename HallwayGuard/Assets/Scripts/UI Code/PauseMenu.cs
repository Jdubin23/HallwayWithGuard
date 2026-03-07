using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;


public class PauseMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private PlayerControls playerControls; //reference to the player controller script to access the input system
    [SerializeField] private PlayerInput playerInput; //recognizes which controls are being used
    private InputAction pauseAction; //recognizes when you press a pause button

    [SerializeField] private GameObject pauseMenuUI; //reveals pause menu and allows it to be disabled
    [SerializeField] private bool GameIsPaused; // allows for me to pause time when the game is paused and unpause when the game is resumed
    [SerializeField] private PlayerController playerScript; // Drag your Player object here in Inspector

    [SerializeField] private InventoryManager Inventory; // Reference to the inventory manager to check if the player has the battery for the icon

    [SerializeField] private GameObject batteryIcon;

    void Awake()
    {
        playerControls = new PlayerControls(); //reference the change between UI and gameplay controls
    }

    private void OnEnable()
    {
        pauseAction = playerControls.UI.Pause; //reference the pause button in the input system
        pauseAction.Enable(); //enable the pause button to be recognized
        pauseAction.performed += OnPauseToggle; //enables listener for the pause button to trigger the OnPauseToggle function when pressed

        if (playerInput != null)
        {
            playerInput.onControlsChanged += OnControlsChanged; //enable the player controls to be recognized
        }
    }
    private void OnDisable()
    {
        pauseAction.Disable();
        pauseAction.performed -= OnPauseToggle; // removes the listener for the pause button when the script is disabled to prevent errors and unintended behavior
        if (playerInput != null)
            playerInput.onControlsChanged -= OnControlsChanged;
    }

private void OnControlsChanged(PlayerInput input)
    {
        // Check if the current scheme is "Gamepad" (or whatever yours is named)
        if (input.currentControlScheme == "Gamepad")
        {
            Cursor.lockState = CursorLockMode.Locked; //Locks the cursor.
            Cursor.visible = false; //hide cursor for better gamepad experience.
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; //unlocks the cursor so it can be used in the pause menu
        Cursor.visible = true; //makes the cursor visible in the pause menu
        }
    }

void OnPauseToggle(InputAction.CallbackContext context)
    {
        if (context.control.device is Pointer) return;
        if (context.performed) //only pause when the button is first pressed, not when it is held down
        {
            // Check if the mouse is clicking a UI element

            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause(playerInput);
            }
        }
    }

    public void Pause(PlayerInput input)
    {
        GameIsPaused = true; //sets the pause state to true
        Time.timeScale = 0f; //pauses the game by setting time scale to 0
        AudioListener.pause = true; //pauses all audio in the game
        pauseMenuUI.SetActive(true); //reveals the pause menu UI

        OnControlsChanged(playerInput);

        EventSystem.current.SetSelectedGameObject(null);
        // GameObject firstButton = pauseMenuUI.GetComponentInChildren<Button>().gameObject;
        // EventSystem.current.SetSelectedGameObject(firstButton);

        playerScript.enabled = false; // This stops Update/FixedUpdate in the player script
        if (Inventory != null)
        {
            batteryIcon.SetActive(Inventory.hasBattery);
        }
    }

  
    public void Resume()
    {
        GameIsPaused = false; //sets the pause state to false
        Time.timeScale = 1f; //resumes the game by setting time scale back to 1
        AudioListener.pause = false; //unpauses all audio in the game

        pauseMenuUI.SetActive(false); //hides the pause menu UI
        
        Cursor.lockState = CursorLockMode.Locked; //unlocks the cursor so it can be used in the pause menu
        Cursor.visible = false; //makes the cursor invisible in the game menu

        playerScript.enabled = true; // This stops Update/FixedUpdate in the player script
        batteryIcon.SetActive(false);
    }

    public void TransitionToMenu()
    {

    Time.timeScale = 1f; //reset time scale to 1 in case we are transitioning to the menu from a paused state, otherwise the menu will be frozen
    AudioListener.pause = false;

    // Change scene to main menu
    SceneManager.LoadScene("MainMenu");
        

    }

}


