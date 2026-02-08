using UnityEngine;
using StateMachineExamples.Scenario.Scripts.Trigger;

/// <summary>
/// Simple player controller for the State Machine package sample.
/// Handles WASD movement and mouse-based camera rotation.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalLookLimit = 80f;
    
    private CharacterController characterController;
    private float verticalVelocity;
    private float cameraPitch;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        InitializeCamera();
    }
    
    private void Start()
    {
        LockCursor();
    }
    
    private void Update()
    {
        HandleMovement();
        HandleCameraRotation();
        HandleCursorToggle();
        HandleInteraction();
    }
    
    private void InitializeCamera()
    {
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogWarning("No camera assigned and no main camera found. Camera rotation will not work.");
            }
        }
    }
    
    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
        float vertical = Input.GetAxisRaw("Vertical");     // W/S
        
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection.Normalize();
        
        Vector3 move = moveDirection * moveSpeed;
        
        ApplyGravity();
        move.y = verticalVelocity;
        
        characterController.Move(move * Time.deltaTime);
    }
    
    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small downward force to keep grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
    
    private void HandleCameraRotation()
    {
        if (cameraTransform == null || Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate camera vertically with clamping
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
    
    private void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            LockCursor();
        }
    }
    
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HandleInteraction()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && Cursor.lockState == CursorLockMode.Locked)
        {
            if (cameraTransform == null) return;

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 3.0f))
            {
                var scenarioObj = hit.collider.GetComponent<ScenarioObject>();
                if (scenarioObj != null)
                {
                    scenarioObj.Interact();
                }
            }
        }
    }
}
