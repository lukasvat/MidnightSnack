using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5.0f;
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float lookUpClamp = 80.0f;
    [SerializeField] float lookDownClamp = -80.0f;

    private float rotationX = 0.0f;
    private Transform cameraTransform;
    private CharacterController characterController;

    void Start()
    {
        // Get necessary components
        characterController = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;

        if (cameraTransform == null)
        {
            Debug.LogError("FirstPersonCamera: No camera found on child object.");
            this.enabled = false;
            return;
        }

        // Lock the cursor to the center of the screen and hide it.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Player Look (Mouse)
        HandleMouseLook();

        // Player Movement (Keyboard)
        HandleMovement();

        // Press Escape to unlock the cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void HandleMouseLook()
    {
        // Get mouse input values.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Vertical rotation (up/down)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, lookDownClamp, lookUpClamp);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Horizontal rotation (left/right)
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        // Get WASD or arrow keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        // Apply movement using the Character Controller
        characterController.Move(moveDirection.normalized * movementSpeed * Time.deltaTime);
    }
}