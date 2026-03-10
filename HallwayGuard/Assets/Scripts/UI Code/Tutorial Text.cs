using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // Cache the main camera transform
        if (Camera.main != null)
            mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // Make the text face the camera
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                mainCameraTransform.rotation * Vector3.up);
        }
    }
}
