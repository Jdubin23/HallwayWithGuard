using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject winMenuUI;
    [SerializeField] private GameObject loseMenuUI;

    [Header("Selection Management")]
    [SerializeField] private GameObject firstWinButton;
    [SerializeField] private GameObject firstLoseButton;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerController playerScript;

    private bool gameHasEnded = false;
    private Vector2 lastMousePosition;

    // Call this for Winning
    public void WinGame()
    {
        TriggerEndSequence(true);
    }

    // Call this for Losing/Death
    public void ActivateLoseUI()
    {
        TriggerEndSequence(false);
    }

    private void TriggerEndSequence(bool won)
    {
        if (gameHasEnded) return;
        gameHasEnded = true;

        // find and stop pause menu ui
        PauseMenu pauseScript = Object.FindFirstObjectByType<PauseMenu>();
        if (pauseScript != null)
        {
            pauseScript.Resume(); // Reset timescale/audio via the pause script's own logic
            pauseScript.isGameOver = true; // This flips the toggle in your PauseMenu inspector
            pauseScript.enabled = false;   // This physically stops the Pause script from running
        }

        // Freeze Game
        Time.timeScale = 0f;
        AudioListener.pause = true;
        if (playerScript != null) playerScript.enabled = false;

        // 3. CLEAR THE EVENT SYSTEM (Crucial for Gamepad/Keyboard)
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        // Show correct ui
        GameObject targetUI = won ? winMenuUI : loseMenuUI;
        targetUI.SetActive(true);

        // show cursor when menu opens
        lastMousePosition = Mouse.current.position.ReadValue();
        
        // Always show and unlock the cursor when the menu opens 
        // so mouse users can immediately click buttons.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerInput.currentControlScheme == "Gamepad")
        {
            HideCursor(); // Only hide it if we are CERTAIN they are using a controller
            GameObject btn = winMenuUI.activeSelf ? firstWinButton : firstLoseButton;
            EventSystem.current.SetSelectedGameObject(btn);
        }
    }

    void Update()
{
    if (!gameHasEnded) return;

    // Detect Mouse Movement
    Vector2 currentMousePos = Mouse.current.position.ReadValue();
    float mouseDelta = Vector2.Distance(currentMousePos, lastMousePosition);

    // If mouse moves even a little (lowered to 0.1f for better sensitivity)
    if (mouseDelta > 0.1f)
    {
        ShowCursor();
        
        // Clear gamepad selection so the "highlight" doesn't stay stuck 
        // while you try to use the mouse
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    lastMousePosition = currentMousePos;

    // Detect which input method is being used
    if (IsNavigating())
    {
        HideCursor();
        
        // Snap selection back to a button if the mouse had cleared it
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            GameObject btn = winMenuUI.activeSelf ? firstWinButton : firstLoseButton;
            EventSystem.current.SetSelectedGameObject(btn);
        }
    }
}

private bool IsNavigating()
{
    // Check Gamepad
    if (Gamepad.current != null)
    {
        if (Gamepad.current.leftStick.ReadValue().magnitude > 0.1f || 
            Gamepad.current.dpad.ReadValue().magnitude > 0.1f) return true;
    }

    // Check Keyboard (Arrows and WASD)
    if (Keyboard.current != null)
    {
        bool arrows = Keyboard.current.upArrowKey.wasPressedThisFrame || 
                      Keyboard.current.downArrowKey.wasPressedThisFrame;
        bool wasd = Keyboard.current.wKey.wasPressedThisFrame || 
                    Keyboard.current.sKey.wasPressedThisFrame;
        
        if (arrows || wasd) return true;
    }

    return false;
}

    private void HideCursor() { Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked; }
    private void ShowCursor() { Cursor.visible = true; Cursor.lockState = CursorLockMode.None; }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }
}