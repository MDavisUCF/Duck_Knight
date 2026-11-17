
using UnityEngine;

public class DuckKnightMovement : MonoBehaviour
{
    [Header("References")]
    public Transform swordTransform;
    public InputManager inputManager;

    [Header("Duck Movement")]
    public float groundHeight = 0f;
    public Vector3 positionOffset = Vector3.zero;
    public float followSpeed = 5f;
    public float keyboardMoveSpeed = 5f;

    [Header("Duck Rotation")]
    public float rotationOffset = 0f;
    public float rotationSpeed = 12f;

    void Update()
    {
        if (inputManager == null)
            return;

        if (GameManager.instance != null &&
            !GameManager.instance.gameRunning)
            return;

        // Mouse Mode: WASD controls Duck Knight.
        if (inputManager.currentMode ==
            InputManager.InputMode.DebugKeyboard)
        {
            UpdateKeyboardMovement();
        }
    }

    void LateUpdate()
    {
        if (swordTransform == null || inputManager == null)
            return;

        if (GameManager.instance != null &&
            !GameManager.instance.gameRunning)
            return;

        // Mocap Mode: Duck Knight follows the physical sword.
        if (inputManager.currentMode ==
            InputManager.InputMode.MocapLive)
        {
            UpdateMocapMovement();
        }
    }

    void UpdateKeyboardMovement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            move += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            move += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            move += Vector3.right;

        transform.position +=
            move.normalized *
            keyboardMoveSpeed *
            Time.deltaTime;

        Vector3 position = transform.position;
        position.y = groundHeight;
        transform.position = position;
    }

    void UpdateMocapMovement()
    {
        Vector3 targetPosition = new Vector3(
            swordTransform.position.x + positionOffset.x,
            groundHeight,
            swordTransform.position.z + positionOffset.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        Vector3 forward = Vector3.ProjectOnPlane(
            swordTransform.forward,
            Vector3.up
        );

        if (forward.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(forward, Vector3.up) *
            Quaternion.Euler(0f, rotationOffset, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
