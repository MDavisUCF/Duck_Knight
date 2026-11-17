
using UnityEngine;

/// <summary>
/// Detects when Duck Knight's sword hits a goblin.
/// Supports both Mouse and Motion Capture modes.
/// </summary>
public class ScoreZone : MonoBehaviour
{
    [Header("Sword Detection")]
    public InputManager inputManager;
    public Collider swordCollider;

    // Prevents the same goblin from being defeated twice.
    private bool hasBeenHit = false;

    private void OnTriggerEnter(Collider other)
    {
        TrySwordHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TrySwordHit(other);
    }

    private void TrySwordHit(Collider other)
    {
        if (hasBeenHit)
            return;

        if (GameManager.instance != null &&
            !GameManager.instance.gameRunning)
            return;

        // Only the sword can defeat goblins.
        if (swordCollider == null || other != swordCollider)
            return;

        // Mouse mode will require an active sword swing.
        // Mocap mode will use valid sword tracking.
        if (inputManager == null || !inputManager.SwordActive)
            return;

        hasBeenHit = true;

        // Play a random goblin death sound.
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayGoblinDeathSound();
        }

        Debug.Log("DUCK KNIGHT: Goblin defeated!");

        gameObject.SetActive(false);
    }
}
