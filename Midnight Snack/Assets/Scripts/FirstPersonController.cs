using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5.0f;
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float lookUpClamp = 80.0f;
    [SerializeField] float lookDownClamp = -80.0f;
    [SerializeField] AudioSource footstepAudioSource; 

    private float rotationX = 0.0f;
    private Transform cameraTransform;
    private CharacterController characterController;

    void Start()
    {
        // Get components
        characterController = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;

        // Lock the cursor to the center of the screen and hide it.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Footstep audio off
        if (footstepAudioSource != null)
        {
            footstepAudioSource.Stop(); 
        }
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
        characterController.SimpleMove(moveDirection.normalized * movementSpeed);

        // Check if the player is moving
        bool isMoving = (horizontalInput != 0 || verticalInput != 0);
    
        // Check if the CharacterController is on the ground
        bool isGrounded = characterController.isGrounded;

        // Trigger footstep audio
        PlayFootstep(isMoving && isGrounded);
    }

    void PlayFootstep(bool isMoving)
    {
        if (isMoving)
        {
            // start playing the audio
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Play(); 
            }
        }
        else
        {
            // stop the audio
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }
    }

    // Called when CharacterController enters a Trigger.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the trigger we entered has the "Monster" tag
        if (other.gameObject.CompareTag("Monster"))
        {
            Debug.Log("Entered the Monster's trigger");
            
            // Call the LoseGame function from GameManager
            GameManager.Instance.LoseGame();
        }

        // Check if it is the entrance trigger
        if (other.gameObject.CompareTag("EntranceTrigger"))
        {
            Debug.Log("Entered the Entrance trigger");
            
            // Call station enter function from GameManager
            GameManager.Instance.OnStationEnter();
        }
    }
}