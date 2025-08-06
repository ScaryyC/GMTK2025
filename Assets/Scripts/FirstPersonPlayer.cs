using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 100f;
    public float fallingSpeed = 9.8f;
    public bool sphericalMovement = false;

    [Header("Camera")]
    public Camera cameraObject;
    public float xSensitivity = 0.2f;
    public float ySensitivity = 0.2f;
    public float maxCameraPitchAngle = 60;
    public float minCameraPitchAngle = -70f;

    [Header("Camera Animation")]
    public Transform birdsEyeViewPosition;
    public float cameraAnimationSpeed = 3.0f;
    public float cameraRotationSpeed = 0.8f;
    
    [Header("Slopes (currently unused)")]
    public float maxSlopeAngle = 70f;
    public float slopeDetectionDistance = 0.3f;

    [Header("UI")]
    public UIManager UIManager;

    [Header("Footsteps")]
    public FootstepSounds footstepSounds;

    Vector2 moveInput;
    Vector2 lookInput;
    private float cameraPitch;
    private Vector3 moveDirection;
    Rigidbody rb;

    Tower currentTower;
    int towersInteracted = 0;

    bool hasControl = true;
    bool movingCamera = false;

    bool gamePaused = false;
    bool gameFinished = false;
    void Awake()
    {
        if (UIManager == null)
        {
            Debug.LogError("UI MANAGER IS NOT HOOKED UP");
        }
        rb = GetComponent<Rigidbody>();
        TakeMouseControl();
        InitializeStartingUI();
    }

    private void OnEnable()
    {
        GameManager.onPathCompleted += RemovePlayerControl;
        GameManager.onFinishPathTracing += ShowWinScreen;
    }

    private void OnDisable()
    {
        GameManager.onPathCompleted -= RemovePlayerControl;
        GameManager.onFinishPathTracing -= ShowWinScreen;
    }

    private void FixedUpdate()
    {
        if (hasControl)
        {
            UpdatePlayerMovement();
            UpdateCameraRotation();
        }
        SetAboveGround();

        if (movingCamera)
        {
            MoveCameraToBirdsEye();
        }
    }

    void UpdatePlayerMovement()
    {
        if (moveInput.magnitude != 0)
        {
            moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
            rb.AddForce(moveDirection.normalized * walkSpeed, ForceMode.Acceleration);
            if (footstepSounds != null) 
            {
                footstepSounds.UpdateFootsteps();
            }
        }
        
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
        hasControl = true;
    }

    void LoseMouseControl()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        hasControl = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void CompleteLoop()
    {
        PlayerPathTracker pathTracker = GameObject.FindAnyObjectByType<PlayerPathTracker>();
        if (pathTracker == null)
        {
            Debug.Log("No path tracker found");
            return;
        }
        if (towersInteracted >= GameManager.GetTowersArrayLength())
        {
            gameFinished = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        pathTracker.CompletePath();
    }

    float extraHeight = 0f;
    void SetAboveGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 2))
        {
            Vector3 point = hit.point;
            point.y += extraHeight;
            transform.position = Vector3.Lerp(transform.position, point, Time.fixedDeltaTime * fallingSpeed);

            if (sphericalMovement)
            {
                Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = slopeRotation;
            }
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
        if (currentTower != null && hasControl)
        {
            currentTower.VisitTower();
            LeaveTower();
            towersInteracted++;
            if (towersInteracted >= GameManager.GetTowersArrayLength())
            {
                UIManager.EnableTopLeftText(true);
                UIManager.SetTopLeftText("Return to your starting position!");
                UIManager.EnableInteractText(true);
                UIManager.SetInteractText("Press Enter to end the level");
                GameManager.onAllTowersOn?.Invoke();
            }
        }
    }

    void LeaveTower()
    {
        UIManager.EnableInteractText(false);
        currentTower = null;
    }

    void RemovePlayerControl()
    {
        hasControl = false;
        StartCameraBirdsEyeTransition();
    }

    void StartCameraBirdsEyeTransition()
    {
        if (birdsEyeViewPosition == null)
        {
            Debug.Log("No birds eye position set");
            return;
        }
        movingCamera = true;
    }

    void MoveCameraToBirdsEye()
    {
        Vector3 currentCameraPosition = cameraObject.transform.position;
        Vector3 camPos = Vector3.Lerp(currentCameraPosition, birdsEyeViewPosition.position, Time.fixedDeltaTime * cameraAnimationSpeed);
        cameraObject.transform.position = camPos;

        cameraPitch = Mathf.LerpAngle(cameraPitch, 90, Time.fixedDeltaTime * cameraRotationSpeed);
        cameraObject.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        if (Mathf.Abs(currentCameraPosition.y - birdsEyeViewPosition.position.y) <= 1)
        {
            GameManager.onStartPathTracing?.Invoke();
            movingCamera = false;
        }
    }

    void InitializeStartingUI()
    {
        UIManager.EnableWinScren(false);
        UIManager.EnableInteractText(false);
        UIManager.EnableTopLeftText(true);
        UIManager.SetTopLeftText("Activate all of the towers!");
    }

    public void ShowWinScreen()
    {
        UIManager.EnableWinScren(true);
    }

    public void PauseGame()
    {
        if (gameFinished)
        {
            return;
        }

        if (!gamePaused)
        {
            gamePaused = true;
            UIManager.EnableTopLeftText(false);
            UIManager.EnablePauseScren(true);
            LoseMouseControl();
            Time.timeScale = 0f;
        }
        else
        {
            UnpauseGame();
        }   
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        gamePaused = false;
        UIManager.EnablePauseScren(false);
        TakeMouseControl();
    }

    public void GoToMainMenu()
    {
        GameManager gm = GameObject.FindAnyObjectByType<GameManager>();
        if (gm == null)
        {
            Debug.Log("No game manager found");
            return;
        }
        Time.timeScale = 1f;
        gm.LoadLevel(0);
    }

    public void NextLevel()
    {
        GameManager gm = GameObject.FindAnyObjectByType<GameManager>();
        if (gm == null)
        {
            Debug.Log("No game manager found");
            return;
        }
        Time.timeScale = 1f;
        gm.LoadNextLevel();
    }

    public void ResetLevel()
    {
        GameManager gm = GameObject.FindAnyObjectByType<GameManager>();
        if (gm == null)
        {
            Debug.Log("No game manager found");
            return;
        }
        Time.timeScale = 1f;
        gm.ReloadLevel();
    }

}
