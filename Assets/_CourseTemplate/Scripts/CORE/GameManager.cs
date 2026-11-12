
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages Duck Knight's score, timer, egg health,
/// damage protection, victory, and game over.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game State")]
    public int score = 0;
    public float roundTime = 90f;
    public bool gameRunning = true;

    [Header("Egg Health")]
    public int maxEggs = 4;
    public int currentEggs = 4;

    [Header("Damage Settings")]
    public float damageImmunityDuration = 1f;

    private float lastDamageTime = -999f;
    private bool resetPending = false;

    public bool IsDamageImmune =>
        Time.time < lastDamageTime + damageImmunityDuration;

    void Awake()
    {
        instance = this;

        currentEggs = maxEggs;
    }

    void Update()
    {
        if (gameRunning)
        {
            roundTime -= Time.deltaTime;

            if (roundTime <= 0f)
            {
                roundTime = 0f;
                WinGame();
            }
        }

        // Press R to restart.
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }

    /// <summary>
    /// Adds points while the round is active.
    /// </summary>
    public void AddScore(int amount)
    {
        if (!gameRunning)
            return;

        score += amount;

        Debug.Log("DUCK KNIGHT: Score = " + score);
    }

    /// <summary>
    /// Attempts to break one egg.
    /// Returns true if damage was actually taken.
    /// </summary>
    public bool TakeDamage()
    {
        if (!gameRunning || currentEggs <= 0)
            return false;

        // Prevent several enemies from breaking eggs at once.
        if (IsDamageImmune)
            return false;

        currentEggs--;
        lastDamageTime = Time.time;

        Debug.Log(
            "DUCK KNIGHT: Egg broken! " +
            currentEggs + " / " + maxEggs + " remaining."
        );

        if (currentEggs <= 0)
        {
            currentEggs = 0;
            LoseGame();
        }

        return true;
    }

    /// <summary>
    /// Called when the player survives the round.
    /// </summary>
    private void WinGame()
    {
        if (!gameRunning)
            return;

        gameRunning = false;

        Debug.Log(
            "DUCK KNIGHT: VICTORY! Final Score: " + score
        );
    }

    /// <summary>
    /// Called when all four eggs are broken.
    /// </summary>
    private void LoseGame()
    {
        if (!gameRunning)
            return;

        gameRunning = false;

        Debug.Log(
            "DUCK KNIGHT: GAME OVER! All eggs broken. Final Score: " + score
        );
    }

    /// <summary>
    /// Plays the reset sound and reloads the scene.
    /// </summary>
    public void ResetGame()
    {
        if (resetPending)
            return;

        resetPending = true;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayResetSound();
        }

        Invoke(nameof(ReloadCurrentScene), 0.65f);
    }

    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}
