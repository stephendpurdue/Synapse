using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;
    
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log($"Move Input: {moveInput}");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log($"Jumping: {context.performed} - Is Grounded: {controller.isGrounded}");
        if (context.performed && controller.isGrounded)
        {
            Debug.Log("We are supposed to jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Reset vertical velocity when grounded
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small value to keep grounded
        }

        // Calculate horizontal movement
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * speed;
        
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        
        // Combine horizontal and vertical movement
        move.y = velocity.y;
        
        // Move character once with combined movement
        controller.Move(move * Time.deltaTime);
    }
}

