
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private const string InputModePrefKey = "LastInputMode";

    public enum InputMode
    {
        DebugKeyboard = 0,
        MocapLive = 2
    }

    [Header("Sword")]
    public Transform swordTransform;
    public Transform mocapTarget;

    [Header("Sword Swing Animation")]
public Transform swordSwingPivot;

    [Header("Duck and Shield")]
    public Transform duckTransform;
    public Transform shieldTransform;
    [Header("Shield Visibility")]
public GameObject shieldModel;

    [Header("Keyboard Combat Stance")]
public Transform swordRestPoint;
public Transform shieldRestPoint;

    [Header("Input Mode")]
    public InputMode currentMode = InputMode.DebugKeyboard;

    [Header("Mouse Sword Controls")]
    public Camera gameplayCamera;
    public float mouseGroundHeight = 0f;
    public float swordReach = 1.2f;
    public float swordHeightOffset = 0.75f;
    public float duckRotationSpeed = 12f;

    [Header("Sword Swing")]
    public float swingDuration = 0.35f;
    public float swingCooldown = 0.15f;
    public float swingStartAngle = -70f;
    public float swingEndAngle = 70f;


[Header("Mocap Play Space Alignment")]
[Range(0f, 360f)]
public float mocapAlignmentY = 180f;

public Vector3 mocapTrackingOrigin = Vector3.zero;
public Vector3 mocapGameOrigin = Vector3.zero;


    [Header("Live Sword Tracking")]
    public Transform liveMocapSource;
    public OptitrackRigidBody liveRigidBody;

    public Vector3 liveMocapPositionOffset = Vector3.zero;
    public Vector3 liveMocapRotationOffset = Vector3.zero;
    public float liveMocapScale = 1f;

    [Header("Live Shield Tracking")]
    public Transform shieldMocapSource;
    public OptitrackRigidBody shieldRigidBody;

    public Vector3 shieldMocapPositionOffset = Vector3.zero;
    public Vector3 shieldMocapRotationOffset = Vector3.zero;
    public float shieldMocapScale = 1f;

    [Header("Tracking Settings")]
    [Range(0.05f, 1f)]
    public float trackingTimeout = 0.25f;

    [Header("Mouse Shield Activation")]
    public float shieldDuration = 5f;

    public bool ShieldActive { get; private set; }
    public bool SwordActive { get; private set; }

    public float ShieldTimeRemaining
    {
        get
        {
            if (!ShieldActive || currentMode == InputMode.MocapLive)
                return 0f;

            return Mathf.Max(0f, shieldEndTime - Time.time);
        }
    }

    private float shieldEndTime;
    private float swingTimer;
    private float nextAllowedSwingTime;
    private bool swinging;

    private Vector3 originalShieldLocalPosition;
    private Quaternion originalShieldLocalRotation;

    void Start()
    {
        if (gameplayCamera == null)
            gameplayCamera = Camera.main;

        if (shieldTransform != null)
        {
            originalShieldLocalPosition = shieldTransform.localPosition;
            originalShieldLocalRotation = shieldTransform.localRotation;
        }

        if (PlayerPrefs.HasKey(InputModePrefKey))
        {
            currentMode =
                (InputMode)PlayerPrefs.GetInt(InputModePrefKey);
        }

        if (!System.Enum.IsDefined(typeof(InputMode), currentMode))
            currentMode = InputMode.DebugKeyboard;

        ShieldActive = false;
        SwordActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            ToggleInputMode();

        bool roundRunning =
            GameManager.instance == null ||
            GameManager.instance.gameRunning;

        if (!roundRunning)
        {
            SwordActive = false;
            ShieldActive = false;
            swinging = false;
            return;
        }

        if (currentMode == InputMode.DebugKeyboard)
        {
            UpdateMouseAttack();
            UpdateMouseShield();
        }
        else
        {
            UpdateLiveSword();
            UpdateLiveShield();
        }
    }

    

void LateUpdate()
{
    UpdateShieldVisibility();

    if (currentMode == InputMode.MocapLive)
    {
        if (swordSwingPivot != null)
            swordSwingPivot.localRotation = Quaternion.identity;

        return;
    }

    if (GameManager.instance != null &&
        !GameManager.instance.gameRunning)
        return;

    UpdateMouseSwordPosition();
    UpdateMouseShieldPosition();
    UpdateSwordSwingAnimation();
}



    // -------------------------
    // MOUSE ATTACKS
    // -------------------------

    void UpdateMouseAttack()
    {
        if (Input.GetMouseButtonDown(0) &&
            !swinging &&
            Time.time >= nextAllowedSwingTime)
        {
            swinging = true;
            swingTimer = 0f;
        }

        if (swinging)
        {
            swingTimer += Time.deltaTime;

            if (swingTimer >= Mathf.Max(0.01f, swingDuration))
            {
                swinging = false;
                nextAllowedSwingTime = Time.time + swingCooldown;
            }
        }

        // Only an actual mouse swing can damage goblins.
        SwordActive = swinging;
    }

   
void UpdateMouseSwordPosition()
{
    if (swordTransform == null ||
        duckTransform == null ||
        swordRestPoint == null ||
        gameplayCamera == null)
        return;

    // Aim Duck Knight toward the mouse cursor.
    Ray ray = gameplayCamera.ScreenPointToRay(
        Input.mousePosition
    );

    Plane groundPlane = new Plane(
        Vector3.up,
        new Vector3(0f, mouseGroundHeight, 0f)
    );

    if (groundPlane.Raycast(ray, out float distance))
    {
        Vector3 mousePoint = ray.GetPoint(distance);

        Vector3 aimDirection =
            mousePoint - duckTransform.position;

        aimDirection.y = 0f;

        if (aimDirection.sqrMagnitude > 0.001f)
        {
Quaternion aimRotation =
    Quaternion.LookRotation(
        aimDirection.normalized,
        Vector3.up
    ) * Quaternion.Euler(0f, 180f, 0f);

            duckTransform.rotation = Quaternion.Slerp(
                duckTransform.rotation,
                aimRotation,
                duckRotationSpeed * Time.deltaTime
            );
        }
    }

    // Resting position is at Duck Knight's front-right.
    // Only rotate the weapon around the duck during a swing.
    float swingAngle = 0f;

    if (swinging)
    {
        float progress = Mathf.Clamp01(
            swingTimer / Mathf.Max(0.01f, swingDuration)
        );

        // Sweep from the right toward the left.
        swingAngle = Mathf.Lerp(
            swingEndAngle,
            swingStartAngle,
            progress
        );
    }

    Quaternion swingRotation = Quaternion.AngleAxis(
        swingAngle,
        Vector3.up
    );

    Vector3 restOffset =
        swordRestPoint.position - duckTransform.position;

    swordTransform.position =
        duckTransform.position +
        swingRotation * restOffset;

    swordTransform.rotation =
        swingRotation * swordRestPoint.rotation;

    if (mocapTarget != null)
    {
        mocapTarget.position = swordTransform.position;
        mocapTarget.rotation = swordTransform.rotation;
    }
}


void UpdateSwordSwingAnimation()
{
    if (swordSwingPivot == null)
        return;

    float sideAngle = 0f;
    float downwardAngle = 0f;

    if (swinging)
    {
        float progress = Mathf.Clamp01(
            swingTimer / Mathf.Max(0.01f, swingDuration)
        );

        if (progress < 0.6f)
        {
            // Fast attack.
            float attackProgress =
                Mathf.SmoothStep(0f, 1f, progress / 0.6f);

            sideAngle = Mathf.Lerp(-55f, 65f, attackProgress);

            downwardAngle = Mathf.Lerp(
                0f,
                65f,
                attackProgress
            );
        }
        else
        {
            // Return to resting position.
            float recoveryProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    (progress - 0.6f) / 0.4f
                );

            sideAngle = Mathf.Lerp(
                65f,
                0f,
                recoveryProgress
            );

            downwardAngle = Mathf.Lerp(
                65f,
                0f,
                recoveryProgress
            );
        }
    }

    swordSwingPivot.localRotation =
        Quaternion.Euler(
            downwardAngle,
            0f,
            sideAngle
        );
}


void UpdateMouseShieldPosition()
{
    if (shieldTransform == null || shieldRestPoint == null)
        return;

    // Shield stays on the duck's front-left.
    // The existing ShieldActive property controls blocking.
    shieldTransform.position = shieldRestPoint.position;
    shieldTransform.rotation = shieldRestPoint.rotation;
}


    // -------------------------
    // MOUSE SHIELD
    // -------------------------

    void UpdateMouseShield()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !ShieldActive)
        {
            ShieldActive = true;
            shieldEndTime = Time.time + shieldDuration;

            if (AudioManager.instance != null)
                AudioManager.instance.PlayShieldActivateSound();
        }

        if (ShieldActive && Time.time >= shieldEndTime)
        {
            ShieldActive = false;

            if (AudioManager.instance != null)
                AudioManager.instance.PlayShieldDeactivateSound();
        }
    }
void UpdateShieldVisibility()
{
    if (shieldModel == null)
        return;

    if (shieldModel.activeSelf != ShieldActive)
    {
        shieldModel.SetActive(ShieldActive);
    }
}
    // -------------------------
    // LIVE SWORD
    // -------------------------


// Converts a tracked position into Duck Knight's game space.
private Vector3 AlignMocapPosition(Vector3 trackedPosition)
{
    Quaternion alignment = Quaternion.Euler(
        0f, mocapAlignmentY, 0f
    );

    return mocapGameOrigin +
        alignment * (trackedPosition - mocapTrackingOrigin);
}

// Converts a tracked rotation into Duck Knight's game space.
private Quaternion AlignMocapRotation(Quaternion trackedRotation)
{
    Quaternion alignment = Quaternion.Euler(
        0f, mocapAlignmentY, 0f
    );

    return alignment * trackedRotation;
}


    void UpdateLiveSword()
    {
        SwordActive = HasLiveTracking();

        if (!SwordActive ||
            swordTransform == null ||
            liveMocapSource == null)
            return;


Vector3 mappedPosition =
    AlignMocapPosition(liveMocapSource.position)
    * liveMocapScale
    + liveMocapPositionOffset;

Quaternion mappedRotation =
    AlignMocapRotation(liveMocapSource.rotation)
    * Quaternion.Euler(liveMocapRotationOffset);


        swordTransform.position = mappedPosition;
        swordTransform.rotation = mappedRotation;

        if (mocapTarget != null)
        {
            mocapTarget.position = mappedPosition;
            mocapTarget.rotation = mappedRotation;
        }
    }

    // -------------------------
    // LIVE SHIELD
    // -------------------------

    void UpdateLiveShield()
    {
        ShieldActive = HasShieldTracking();

        if (!ShieldActive ||
            shieldTransform == null ||
            shieldMocapSource == null)
            return;

        
shieldTransform.position =
    AlignMocapPosition(shieldMocapSource.position)
    * shieldMocapScale
    + shieldMocapPositionOffset;

shieldTransform.rotation =
    AlignMocapRotation(shieldMocapSource.rotation)
    * Quaternion.Euler(shieldMocapRotationOffset);

    }

    // -------------------------
    // TRACKING VALIDATION
    // -------------------------

    public bool HasLiveTracking()
    {
        return HasTracking(liveRigidBody);
    }

    public bool HasShieldTracking()
    {
        return HasTracking(shieldRigidBody);
    }

    private bool HasTracking(OptitrackRigidBody rigidBody)
    {
        if (rigidBody == null ||
            rigidBody.StreamingClient == null)
            return false;

        OptitrackRigidBodyState state =
            rigidBody.StreamingClient.GetLatestRigidBodyState(
                rigidBody.RigidBodyId,
                false
            );

        if (state == null)
            return false;

        float age = state.DeliveryTimestamp.AgeSeconds;

        return age >= 0f && age <= trackingTimeout;
    }

    // -------------------------
    // MODE SWITCHING
    // -------------------------

    void ToggleInputMode()
    {
        if (currentMode == InputMode.DebugKeyboard)
        {
            currentMode = InputMode.MocapLive;
        }
        else
        {
            currentMode = InputMode.DebugKeyboard;

            if (shieldTransform != null)
            {
                shieldTransform.localPosition =
                    originalShieldLocalPosition;

                shieldTransform.localRotation =
                    originalShieldLocalRotation;
            }
        }

        swinging = false;
        SwordActive = false;
        ShieldActive = false;

        PlayerPrefs.SetInt(InputModePrefKey, (int)currentMode);
        PlayerPrefs.Save();

        if (AudioManager.instance != null)
            AudioManager.instance.PlayInputModeSound();
    }

    public string GetInputModeName()
    {
        return currentMode == InputMode.DebugKeyboard
            ? "Mouse"
            : "Motion Capture";
    }
}
