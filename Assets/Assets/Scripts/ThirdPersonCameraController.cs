using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLerpSpeed = 20f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 15f;
    
    private PlayerControls controls;
    
    private CinemachineCamera cam;
    private CinemachineOrbitalFollow orbital;
    private Vector2 scrollDelta;

    private float targetZoom;
    private float currentZoom;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = new PlayerControls();
        controls.Enable();
        controls.CameraControls.MouseZoom.performed += HandleMouseScroll;

        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<CinemachineCamera>();
        if (cam == null)
        {
            Debug.LogError("CinemachineCamera component not found!");
            enabled = false;
            return;
        }

        orbital = cam.GetComponent<CinemachineOrbitalFollow>();
        if (orbital == null)
        {
            Debug.LogError("CinemachineOrbitalFollow component not found!");
            enabled = false;
            return;
        }

        targetZoom = currentZoom = orbital.Radius;
    }

    private void HandleMouseScroll(InputAction.CallbackContext obj)
    {
        scrollDelta = obj.ReadValue<Vector2>();
        Debug.Log($"Mouse is scrolling. Value: {scrollDelta}");
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollDelta.y != 0)
        {
            if (orbital != null)
            {
                targetZoom = Mathf.Clamp(orbital.Radius - scrollDelta.y * zoomSpeed, minDistance, maxDistance);
                scrollDelta = Vector2.zero;
            }
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);
        orbital.Radius = currentZoom;
    }

    void OnDestroy() // Fix Memory Leaks
    {
    if (controls != null)
    {
        controls.CameraControls.MouseZoom.performed -= HandleMouseScroll;
        controls.Disable();
        controls.Dispose();
    }
}
}
