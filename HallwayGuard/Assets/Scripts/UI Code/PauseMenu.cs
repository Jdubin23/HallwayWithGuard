using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private PlayerControls playerControls; //reference to the player controller script to access the input system
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
    }
    private void OnDisable()
    {
        pauseAction.Disable();
        pauseAction.performed -= OnPauseToggle; // removes the listener for the pause button when the script is disabled to prevent errors and unintended behavior

    }


void OnPauseToggle(InputAction.CallbackContext context)
    {
        Debug.Log("Toggle Fired");
        if (context.performed) //only pause when the button is first pressed, not when it is held down
        {
            // Check if the mouse is clicking a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Exit the function so we don't toggle pause/resume via script
        }
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        GameIsPaused = true; //sets the pause state to true
        Time.timeScale = 0f; //pauses the game by setting time scale to 0
        AudioListener.pause = true; //pauses all audio in the game
        
        pauseMenuUI.SetActive(true); //reveals the pause menu UI

        Cursor.lockState = CursorLockMode.None; //unlocks the cursor so it can be used in the pause menu
        Cursor.visible = true; //makes the cursor visible in the pause menu

        playerScript.enabled = false; // This stops Update/FixedUpdate in the player script
        if (Inventory != null)
        {
            batteryIcon.SetActive(Inventory.hasBattery);
        }
    }

  
    public void Resume()
    {
        Debug.Log("Resume button was clicked!");
        GameIsPaused = false; //sets the pause state to false
        Time.timeScale = 1f; //resumes the game by setting time scale back to 1
        AudioListener.pause = false; //unpauses all audio in the game

        pauseMenuUI.SetActive(false); //hides the pause menu UI
        
        Cursor.lockState = CursorLockMode.Locked; //unlocks the cursor so it can be used in the pause menu
        Cursor.visible = false; //makes the cursor invisible in the game menu

        playerScript.enabled = true; // This stops Update/FixedUpdate in the player script
        batteryIcon.SetActive(false);
    }

}


