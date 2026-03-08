using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuNavigation : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera menuCamera;
    [SerializeField] private float travelTime = 1.5f;

    [Header("Transition Targets")]
    [SerializeField] private Transform gameStartTarget;
    [SerializeField] private Transform settingsTarget;
    [SerializeField] private Transform creditsTarget;
    [SerializeField] private Transform quitTarget; // Slot for the Quit camera move

    [Header("UI Transition")]
    [SerializeField] private CanvasGroup mainMenuUI; 

    private bool isTransitioning = false;

    // --- BRIDGE FUNCTIONS ---

    public void LoadGameScene(string sceneName) => ExecuteTransition(gameStartTarget, sceneName);
    public void LoadSettingsScene(string sceneName) => ExecuteTransition(settingsTarget, sceneName);
    public void LoadCreditsScene(string sceneName) => ExecuteTransition(creditsTarget, sceneName);
    public void LoadQuitScene(string sceneName) => QuitSequence();


    // NEW: Quit Bridge
    public void QuitGame()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(QuitSequence());
    }

    // --- THE SEQUENCES ---

    private void ExecuteTransition(Transform target, string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(SceneTransitionSequence(target, sceneName));
    }

    private IEnumerator SceneTransitionSequence(Transform target, string sceneName)
    {
        HideUI();

        if (target != null)
            yield return StartCoroutine(CameraMove(target));

        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator QuitSequence()
    {
        HideUI();

        // Move to the quit target (e.g., flying out to space)
        if (quitTarget != null)
            yield return StartCoroutine(CameraMove(quitTarget));

        yield return new WaitForSeconds(0.2f);

        Debug.Log("Quitting Game...");
        Application.Quit();

        // Note: Application.Quit() does not work in the Unity Editor.
        // This line makes it "Quit" while you are testing in the editor:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void HideUI()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.alpha = 0;
            mainMenuUI.interactable = false;
            mainMenuUI.blocksRaycasts = false;
        }
    }

    private IEnumerator CameraMove(Transform target)
    {
        Vector3 startPos = menuCamera.transform.position;
        Quaternion startRot = menuCamera.transform.rotation;
        float elapsed = 0;

        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / travelTime);
            float easedT = t * t * (3f - 2f * t);

            menuCamera.transform.position = Vector3.Lerp(startPos, target.position, easedT);
            menuCamera.transform.rotation = Quaternion.Lerp(startRot, target.rotation, easedT);

            yield return null;
        }

        menuCamera.transform.position = target.position;
        menuCamera.transform.rotation = target.rotation;
    }
}