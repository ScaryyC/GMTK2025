using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 100f;

    [Header("Camera")]
    public GameObject cameraObject;
    public float xSensitivity = 0.2f;
    public float ySensitivity = 0.2f;
    public float maxCameraPitchAngle = 60;
    public float minCameraPitchAngle = -70f;

    Vector2 moveInput;
    Vector2 lookInput;
    private float cameraPitch;
    private Vector3 moveDirection;

    Transform transformComponent;
    Rigidbody rb;

    void Start()
    {
        transformComponent = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        TakeMouseControl();
    }

    
    void Update()
    {
        UpdateCameraRotation();
    }

    private void FixedUpdate()
    {
        UpdatePlayerMovement();
    }

    void UpdatePlayerMovement()
    {
        moveDirection = transformComponent.forward * moveInput.y + transformComponent.right * moveInput.x;
        rb.AddForce(moveDirection.normalized * walkSpeed, ForceMode.Force);
    }

    void UpdateCameraRotation()
    {
        if (cameraObject == null)
        {
            Debug.Log("Theres no camera!");
            return;
        }

        if (transformComponent == null)
        {
            Debug.Log("Could not get transform");
            return;
        }

        float mouseX = lookInput.x * xSensitivity;
        float mouseY = lookInput.y * ySensitivity;
        
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minCameraPitchAngle, maxCameraPitchAngle);
        
        cameraObject.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        transformComponent.Rotate(Vector3.up * mouseX);
    }

    void TakeMouseControl()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
