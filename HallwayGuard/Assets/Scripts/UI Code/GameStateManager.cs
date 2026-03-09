using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

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
        StartCoroutine(TriggerEndSequence(true));
    }

    // Call this for Losing/Death
    public void ActivateLoseUI()
    {
        StartCoroutine(TriggerEndSequence(false));
    }

    private IEnumerator TriggerEndSequence(bool won)
    {
        if (gameHasEnded) yield break;
        gameHasEnded = true;

        // 1. Handle Pause Menu
        PauseMenu pauseScript = Object.FindFirstObjectByType<PauseMenu>();
        if (pauseScript != null)
        {
            pauseScript.Resume();
            pauseScript.isGameOver = true;
            pauseScript.enabled = false;
        }

        // 2. Freeze Game
        Time.timeScale = 0f;
        AudioListener.pause = true;
        if (playerScript != null) playerScript.enabled = false;

        // 3. Clear the Event System immediately so no "ghost" highlights exist
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        // 4. Show correct UI
        GameObject targetUI = won ? winMenuUI : loseMenuUI;
        targetUI.SetActive(true);

        // 5. Setup Cursor
        lastMousePosition = Mouse.current.position.ReadValue();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // --- THE FIX: WAIT BEFORE ENABLING BUTTON INTERACTION ---
        // We use WaitForSecondsRealtime because Time.timeScale is 0
        yield return new WaitForSecondsRealtime(0.15f);

        // 6. Final Selection (Now happens after the tiny delay)
        if (playerInput.currentControlScheme == "Gamepad")
        {
            HideCursor();
            GameObject btn = won ? firstWinButton : firstLoseButton;
            EventSystem.current.SetSelectedGameObject(btn);
        }
    }

    void Update()
    {
        if (!gameHasEnded) return;

        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        float mouseDelta = Vector2.Distance(currentMousePos, lastMousePosition);

        if (mouseDelta > 0.1f)
        {
            ShowCursor();
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        lastMousePosition = currentMousePos;

        if (IsNavigating())
        {
            HideCursor();
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