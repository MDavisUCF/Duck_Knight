
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

    [Header("Input Mode")]
    public InputMode currentMode = InputMode.DebugKeyboard;

    [Header("Debug Keyboard")]
    public float moveSpeed = 5f;

    [Header("Live Sword Tracking")]
    public Transform liveMocapSource;
    public OptitrackRigidBody liveRigidBody;

    public Vector3 liveMocapPositionOffset = Vector3.zero;
    public Vector3 liveMocapRotationOffset = Vector3.zero;
    public float liveMocapScale = 1f;

    [Range(0.05f, 1f)]
    public float trackingTimeout = 0.25f;

    [Header("Shield Debug Activation")]
    public float shieldDuration = 5f;

    public bool ShieldActive { get; private set; }
    public bool SwordActive { get; private set; }

    public float ShieldTimeRemaining
    {
        get
        {
            if (!ShieldActive)
                return 0f;

            return Mathf.Max(0f, shieldEndTime - Time.time);
        }
    }

    private float shieldEndTime;
    private float keyboardHeight;

    void Start()
    {
        if (swordTransform != null)
        {
            keyboardHeight = swordTransform.position.y;

            if (mocapTarget != null)
            {
                mocapTarget.position = swordTransform.position;
                mocapTarget.rotation = swordTransform.rotation;
            }
        }

        if (PlayerPrefs.HasKey(InputModePrefKey))
        {
            currentMode =
                (InputMode)PlayerPrefs.GetInt(InputModePrefKey);
        }

        if (!System.Enum.IsDefined(typeof(InputMode), currentMode))
        {
            currentMode = InputMode.DebugKeyboard;
        }

        ShieldActive = false;
        shieldEndTime = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleInputMode();
        }

        bool roundRunning =
            GameManager.instance == null ||
            GameManager.instance.gameRunning;

        if (!roundRunning)
        {
            SwordActive = false;
            ShieldActive = false;
            return;
        }

        if (currentMode == InputMode.DebugKeyboard)
        {
            UpdateKeyboardSword();
            UpdateKeyboardShield();
        }
        else
        {
            UpdateLiveSword();

            // Independent shield tracking will be added
            // when the second OptiTrack rigid body is ready.
            ShieldActive = false;
        }
    }

    void UpdateKeyboardSword()
    {
        if (swordTransform == null)
        {
            SwordActive = false;
            return;
        }

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            move += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            move += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            move += Vector3.right;

        Vector3 newPosition = swordTransform.position;

        newPosition +=
            move.normalized * moveSpeed * Time.deltaTime;

        // Maintain a fixed height during keyboard testing.
        newPosition.y = keyboardHeight;

        swordTransform.position = newPosition;

        if (mocapTarget != null)
        {
            mocapTarget.position = swordTransform.position;
            mocapTarget.rotation = swordTransform.rotation;
        }

        // The sword is always active in keyboard mode.
        SwordActive = true;
    }

    void UpdateKeyboardShield()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !ShieldActive)
        {
            ShieldActive = true;
            shieldEndTime = Time.time + shieldDuration;

            Debug.Log("DUCK KNIGHT: Shield activated!");
        }

        if (ShieldActive && Time.time >= shieldEndTime)
        {
            ShieldActive = false;

            Debug.Log("DUCK KNIGHT: Shield expired.");
        }
    }

    void UpdateLiveSword()
    {
        SwordActive = HasLiveTracking();

        if (!SwordActive ||
            swordTransform == null ||
            liveMocapSource == null)
        {
            return;
        }

        Vector3 mappedPosition =
            liveMocapSource.position * liveMocapScale
            + liveMocapPositionOffset;

        Quaternion mappedRotation =
            liveMocapSource.rotation *
            Quaternion.Euler(liveMocapRotationOffset);

        swordTransform.position = mappedPosition;
        swordTransform.rotation = mappedRotation;

        if (mocapTarget != null)
        {
            mocapTarget.position = mappedPosition;
            mocapTarget.rotation = mappedRotation;
        }
    }

    public bool HasLiveTracking()
    {
        if (liveRigidBody == null ||
            liveRigidBody.StreamingClient == null)
        {
            return false;
        }

        OptitrackRigidBodyState state =
            liveRigidBody.StreamingClient.GetLatestRigidBodyState(
                liveRigidBody.RigidBodyId,
                false
            );

        if (state == null)
            return false;

        float age = state.DeliveryTimestamp.AgeSeconds;

        return age >= 0f && age <= trackingTimeout;
    }

    void ToggleInputMode()
    {
        if (currentMode == InputMode.DebugKeyboard)
        {
            currentMode = InputMode.MocapLive;
        }
        else
        {
            currentMode = InputMode.DebugKeyboard;

            if (swordTransform != null)
                keyboardHeight = swordTransform.position.y;
        }

        ShieldActive = false;

        PlayerPrefs.SetInt(InputModePrefKey, (int)currentMode);
        PlayerPrefs.Save();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayInputModeSound();
        }

        Debug.Log("Input Mode: " + currentMode);
    }

    public string GetInputModeName()
    {
        return currentMode.ToString();
    }
}
