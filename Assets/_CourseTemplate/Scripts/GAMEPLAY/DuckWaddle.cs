
using UnityEngine;

public class DuckWaddle : MonoBehaviour
{
    [Header("Movement Reference")]
    public Transform duckKnight;

    [Header("Waddle Settings")]
    public float waddleSpeed = 10f;
    public float waddleAngle = 8f;
    public float bobHeight = 0.08f;
    public float movementThreshold = 0.01f;

    [Header("Smoothing")]
    public float transitionSpeed = 8f;

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Vector3 lastDuckPosition;

    private float waddleTime;
    private float movementBlend;

    void Start()
    {
        startingPosition = transform.localPosition;
        startingRotation = transform.localRotation;

        if (duckKnight != null)
            lastDuckPosition = duckKnight.position;
    }

    void LateUpdate()
    {
        if (duckKnight == null)
            return;

        // Detect horizontal movement of Duck Knight.
        Vector3 movement = duckKnight.position - lastDuckPosition;
        movement.y = 0f;

        float movementSpeed = movement.magnitude /
            Mathf.Max(Time.deltaTime, 0.0001f);

        bool isMoving = movementSpeed > movementThreshold;

        lastDuckPosition = duckKnight.position;

        // Smoothly transition between idle and waddling.
        float targetBlend = isMoving ? 1f : 0f;

        movementBlend = Mathf.MoveTowards(
            movementBlend,
            targetBlend,
            transitionSpeed * Time.deltaTime
        );

        if (isMoving)
            waddleTime += Time.deltaTime * waddleSpeed;

        // Rock the duck from side to side.
        float tilt = Mathf.Sin(waddleTime) *
            waddleAngle * movementBlend;

        // Slight bounce with each step.
        float bob = Mathf.Abs(Mathf.Sin(waddleTime)) *
            bobHeight * movementBlend;

        transform.localRotation =
            startingRotation * Quaternion.Euler(0f, 0f, tilt);

        transform.localPosition =
            startingPosition + Vector3.up * bob;
    }
}
