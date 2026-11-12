
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    [Header("References")]
    public Transform duckKnight;
    public InputManager inputManager;
    public Collider shieldCollider;

    [Header("Fireball Movement")]
    public float moveSpeed = 3f;
    public float hitDistance = 0.55f;

    [Header("Shield Block")]
    public int blockPoints = 1;

    private Vector3 travelDirection;
    private Collider fireballCollider;
    private bool resolved = false;

    void Start()
    {
        fireballCollider = GetComponent<Collider>();

        // Aim at Duck Knight's position when the fireball launches.
        if (duckKnight != null)
        {
            Vector3 target = duckKnight.position;
            target.y = transform.position.y;

            travelDirection = (target - transform.position).normalized;
        }
    }

    public void Initialize(Transform target)
{
    duckKnight = target;

    Vector3 targetPosition = duckKnight.position;
    targetPosition.y = transform.position.y;

    travelDirection =
        (targetPosition - transform.position).normalized;
}

    void Update()
    {
        if (resolved || duckKnight == null)
            return;

        if (GameManager.instance != null &&
            !GameManager.instance.gameRunning)
            return;

        // Travel toward the position targeted at launch.
        transform.position += travelDirection * moveSpeed * Time.deltaTime;

        // Check shield interception before checking player damage.
        if (IsBlockedByShield())
        {
            ResolveShieldBlock();
            return;
        }

        // Damage the duck if the fireball reaches it.
        Vector3 duckPosition = duckKnight.position;
        duckPosition.y = transform.position.y;

        if (Vector3.Distance(transform.position, duckPosition)
            <= hitDistance)
        {
            if (GameManager.instance != null)
                GameManager.instance.TakeDamage();

            Debug.Log("DUCK KNIGHT: Fireball hit the duck!");

            ResolveFireball();
        }
    }

    private bool IsBlockedByShield()
    {
        if (inputManager == null ||
            shieldCollider == null ||
            fireballCollider == null)
            return false;

        if (!inputManager.ShieldActive)
            return false;

        if (!shieldCollider.gameObject.activeInHierarchy ||
            !shieldCollider.enabled)
            return false;

        return fireballCollider.bounds.Intersects(
            shieldCollider.bounds
        );
    }

    private void ResolveShieldBlock()
    {
        if (resolved)
            return;

        resolved = true;

        if (GameManager.instance != null)
            GameManager.instance.AddScore(blockPoints);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayDangerSound();

        Debug.Log("DUCK KNIGHT: Fireball blocked! +1 point.");

        ResolveFireball();
    }

    private void ResolveFireball()
    {
        resolved = true;
        gameObject.SetActive(false);
    }
}
