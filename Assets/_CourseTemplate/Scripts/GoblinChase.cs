
using UnityEngine;

public class GoblinChase : MonoBehaviour
{
    [Header("Target")]
    public Transform duckKnight;

    [Header("Movement")]
    public float moveSpeed = 0.9f;
    public float reachDistance = 0.65f;

    private DuckKnightShield duckShield;
    private InputManager inputManager;
    private Collider shieldCollider;
    private Collider goblinCollider;

    [Header("Ground Detection")]
public LayerMask groundLayer;
public float groundRayHeight = 5f;
public float groundRayDistance = 20f;
public float groundOffset = 0f;

    void Start()
    {
        goblinCollider = GetComponent<Collider>();

        // Find the shield attached to Duck Knight.
        if (duckKnight != null)
        {
            duckShield = duckKnight.GetComponent<DuckKnightShield>();

            if (duckShield != null)
            {
                inputManager = duckShield.inputManager;

                if (duckShield.shieldVisual != null)
                {
                    shieldCollider =
                        duckShield.shieldVisual.GetComponent<Collider>();
                }
            }
        }
    }

    void Update()
    {
        if (duckKnight == null)
            return;

        if (GameManager.instance != null &&
            !GameManager.instance.gameRunning)
            return;

       Vector3 targetPosition = duckKnight.position;

// Move horizontally toward Duck Knight.
Vector3 horizontalTarget = new Vector3(
    targetPosition.x,
    transform.position.y,
    targetPosition.z
);

transform.position = Vector3.MoveTowards(
    transform.position,
    horizontalTarget,
    moveSpeed * Time.deltaTime
);

// Find the terrain directly beneath the goblin.
Vector3 rayOrigin = transform.position +
    Vector3.up * groundRayHeight;

if (Physics.Raycast(
    rayOrigin,
    Vector3.down,
    out RaycastHit groundHit,
    groundRayDistance,
    groundLayer,
    QueryTriggerInteraction.Ignore))
{
    Vector3 position = transform.position;
    position.y = groundHit.point.y + groundOffset;
    transform.position = position;
}
        // Rotate the goblin toward Duck Knight.
Vector3 direction = targetPosition - transform.position;

if (direction.sqrMagnitude > 0.001f)
{
    Quaternion targetRotation = Quaternion.LookRotation(direction);

    transform.rotation = Quaternion.Slerp(
        transform.rotation,
        targetRotation,
        8f * Time.deltaTime
    );
}

        // Check whether the active shield intercepts the goblin.
        if (IsBlockedByShield())
        {
            Debug.Log("DUCK KNIGHT: Goblin blocked by shield!");

            // Blocking prevents damage but does not award points.
            gameObject.SetActive(false);
            return;
        }

        // If the goblin reaches Duck Knight without being blocked,
        // break an egg.
Vector3 distanceToDuck = duckKnight.position - transform.position;
distanceToDuck.y = 0f;

if (distanceToDuck.magnitude <= reachDistance)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.TakeDamage();
            }

            Debug.Log("DUCK KNIGHT: Goblin attacked!");

            gameObject.SetActive(false);
        }
    }

    private bool IsBlockedByShield()
    {
        if (inputManager == null ||
            shieldCollider == null ||
            goblinCollider == null)
        {
            return false;
        }

        // Shield must be activated.
        if (!inputManager.ShieldActive)
            return false;

        // Shield object must actually be visible and enabled.
        if (!shieldCollider.gameObject.activeInHierarchy ||
            !shieldCollider.enabled)
        {
            return false;
        }

        // For our current box-shaped prototype,
        // check whether the collision bounds overlap.
        return goblinCollider.bounds.Intersects(
            shieldCollider.bounds
        );
    }
}
