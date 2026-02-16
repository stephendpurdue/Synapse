using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLerpSpeed = 20f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 15f;
    
    private PlayerControls controls;
    
    private CinemachineCamera cam;
    private CinemachineOrbitalFollow orbital;
    private Cinemachine brain;
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
        orbital = cam.GetComponent<CinemachineOrbialFollow>();

        targetZoom = currentZoom = orbital.Radius;
    }

    private void HandleMouseScroll(InputAction.CallbackContext obj)
    {
        scrollDelta = context.ReadValue<Vector2>();
        Debug.Log($"Mouse is scrolling. Value: {scrollDelta}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
