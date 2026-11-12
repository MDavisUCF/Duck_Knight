
using UnityEngine;

/// <summary>
/// Detects when Duck Knight's sword hits a goblin.
/// Only the sword can damage goblins.
/// </summary>
public class ScoreZone : MonoBehaviour
{
    [Header("Sword Detection")]
    public InputManager inputManager;
    public Collider swordCollider;

    [Header("Scoring")]
    public int scoreValue = 1;

    // Prevents repeated scoring from the same goblin.
    private bool hasBeenHit = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only the sword can damage this goblin.
        if (swordCollider == null || other != swordCollider)
            return;

        // The sword must be active.
        if (inputManager == null || !inputManager.SwordActive)
            return;

        // Don't award points twice.
        if (hasBeenHit)
            return;

        hasBeenHit = true;

        // Award points.
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scoreValue);
        }

        // Play the score sound.
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayScoreSound();
        }

        Debug.Log("DUCK KNIGHT: Goblin defeated!");

        // Hide the goblin after a successful hit.
        gameObject.SetActive(false);
    }
}
