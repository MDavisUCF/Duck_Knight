
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

        // Keep goblins moving along the ground.
        targetPosition.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
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
        if (Vector3.Distance(transform.position, targetPosition)
            <= reachDistance)
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
