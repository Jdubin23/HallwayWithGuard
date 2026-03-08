using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; 
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Input System References")]
    private PlayerControls playerControls;
    [SerializeField] private PlayerInput playerInput; 
    private InputAction pauseAction; 
    [SerializeField] private PlayerController playerScript; 

    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuUI; 
    [SerializeField] private GameObject SettingsMenu; 
    [SerializeField] private GameObject winMenuUI;
    [SerializeField] private GameObject loseMenuUI;
    [SerializeField] private GameObject firstSettingsButton;
    public GameObject menuButtonToSelect; // 'Settings' button on the previous menu

    [Header("Check Game State")]
    [SerializeField] private bool GameIsPaused; 
    public bool isGameOver = false;    

    [Header("UI Selection")]
    [SerializeField] private GameObject firstButton; // The 'Resume' button 
    private Vector2 lastMousePosition;

    [SerializeField] private InventoryManager Inventory; 
    [SerializeField] private GameObject batteryIcon;

    void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        pauseAction = playerControls.UI.Pause; 
        pauseAction.Enable(); 
        pauseAction.performed += OnPauseToggle; 

        if (playerInput != null)
        {
            playerInput.onControlsChanged += OnControlsChanged; 
        }
    }
    public void OpenSettings()
{
    SettingsMenu.SetActive(true);
    pauseMenuUI.SetActive(false); // Hide the main buttons
    EventSystem.current.SetSelectedGameObject(firstSettingsButton);
}
public void CloseSettings()
{
    // Save Changes
    PlayerPrefs.Save();

    // Swap Panels
    SettingsMenu.SetActive(false);
    pauseMenuUI.SetActive(true);

    // Swap focus from settings buttons to main menu buttons
    // This prevents the "lost highlight" bug on controllers.
    if (EventSystem.current != null)
    {
        EventSystem.current.SetSelectedGameObject(menuButtonToSelect);
    }
}
    private void OnDisable()
    {
        pauseAction.Disable();
        pauseAction.performed -= OnPauseToggle; 
        if (playerInput != null)
            playerInput.onControlsChanged -= OnControlsChanged;
    }

    void Update()
    {
        if (!GameIsPaused) return;

        // Detect if mouse moves while paused
        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        if (Vector2.Distance(currentMousePos, lastMousePosition) > 2.0f)
        {
            ShowCursor();
            // If mouse moves, clear gamepad selection so they don't fight
            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
        lastMousePosition = currentMousePos;

        // Detect if Gamepad/Arrows are used while paused
        if (IsNavigating())
        {
            HideCursor();
            // Ensure a button is selected for the controller
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(firstButton);
            }
        }
    }

    private bool IsNavigating()
{
    // Check Gamepad Stick/DPad
    if (Gamepad.current != null)
    {
        if (Gamepad.current.leftStick.ReadValue().magnitude > 0.1f || 
            Gamepad.current.dpad.ReadValue().magnitude > 0.1f) 
            return true;
    }

    // Check Keyboard Keys
    if (Keyboard.current != null)
    {
        // WASD
        bool wasd = Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed || 
                    Keyboard.current.sKey.isPressed || Keyboard.current.dKey.isPressed;

        // Arrows
        bool arrows = Keyboard.current.upArrowKey.isPressed || Keyboard.current.downArrowKey.isPressed || 
                      Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed;

        if (wasd || arrows) return true;
    }

    return false;
}

    private void OnControlsChanged(PlayerInput input)
    {
        if (input.currentControlScheme == "Gamepad")
        {
            HideCursor();
        }
        else
        {
            ShowCursor();
        }
    }

    void OnPauseToggle(InputAction.CallbackContext context)
    {
if (isGameOver) return;
       if (SettingsMenu != null && SettingsMenu.activeSelf) 
    {
        return; 
    }

    if (context.performed)
    {
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
        GameIsPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        pauseMenuUI.SetActive(true);

        // Capture mouse pos so we don't 'move' it immediately on pause
        lastMousePosition = Mouse.current.position.ReadValue();

        // Check current device to decide if we show cursor or select button
        if (playerInput.currentControlScheme == "Gamepad")
        {
            HideCursor();
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
        else
        {
            ShowCursor();
            EventSystem.current.SetSelectedGameObject(null);
        }

        playerScript.enabled = false;
        if (Inventory != null) batteryIcon.SetActive(Inventory.hasBattery);
    }

    public void Resume()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        pauseMenuUI.SetActive(false);

        // Always lock cursor when going back to gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerScript.enabled = true;
        batteryIcon.SetActive(false);
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void TransitionToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }
}
