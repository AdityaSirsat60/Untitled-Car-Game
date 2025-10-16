using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera targetCamera;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (targetCamera != null)
        {
            // Make the object face the camera
            transform.LookAt(transform.position + targetCamera.transform.forward);
        }
    }
}
