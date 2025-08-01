using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 100f;
    public float fallingSpeed = 9.8f;

    [Header("Camera")]
    public GameObject cameraObject;
    public float xSensitivity = 0.2f;
    public float ySensitivity = 0.2f;
    public float maxCameraPitchAngle = 60;
    public float minCameraPitchAngle = -70f;
    
    [Header("Slopes (currently unused)")]
    public float maxSlopeAngle = 70f;
    public float slopeDetectionDistance = 0.3f;

    [Header("UI")]
    public UIManager UIManager;

    Vector2 moveInput;
    Vector2 lookInput;
    private float cameraPitch;
    private Vector3 moveDirection;
    Rigidbody rb;

    Tower currentTower;
    int towersInteracted = 0;

    void Start()
    {
        if (UIManager == null)
        {
            Debug.LogError("UI MANAGER IS NOT HOOKED UP");
        }
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
        SetAboveGround();
    }

    void UpdatePlayerMovement()
    {
        moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        rb.AddForce(moveDirection.normalized * walkSpeed, ForceMode.Force);
    }

    void UpdateCameraRotation()
    {
        if (cameraObject == null)
        {
            Debug.Log("Theres no camera!");
            return;
        }

        float mouseX = lookInput.x * xSensitivity;
        float mouseY = lookInput.y * ySensitivity;
        
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minCameraPitchAngle, maxCameraPitchAngle);
        
        cameraObject.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        transform.Rotate(Vector3.up * mouseX);
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

    float extraHeight = 0f;
    void SetAboveGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit))
        {
            Vector3 point = hit.point;
            point.y += extraHeight;
            transform.position = Vector3.Lerp(transform.position, point, Time.fixedDeltaTime * fallingSpeed);
            //Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            //transform.rotation = slopeRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Tower")
        {
            UIManager.EnableInteractText(true);
            currentTower = other.gameObject.GetComponent<Tower>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Tower")
        {
            LeaveTower();
        }
    }

    public void InteractWithTower()
    {
        if (currentTower != null)
        {
            currentTower.VisitTower();
            LeaveTower();
            towersInteracted++;
            if (towersInteracted >= GameManager.GetTowersArrayLength())
            {
                GameManager.onAllTowersOn?.Invoke();
            }
        }
    }

    void LeaveTower()
    {
        UIManager.EnableInteractText(false);
        currentTower = null;
    }

    void OnAllTowersInteracted()
    {
        UIManager.EnableTopLeftText(true);
    }

}
