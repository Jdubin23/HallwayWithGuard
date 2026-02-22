using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{public string sceneToLoad;

    public void LoadScene()
    {SceneManager.LoadScene(sceneToLoad);}}