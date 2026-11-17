
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Manages Duck Knight's round, egg health,
/// victory, defeat, and mocap preparation.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private const string InputModePrefKey = "LastInputMode";

    [Header("Game State")]
    public int score = 0;
    public float roundTime = 60f;
    public bool gameRunning = true;

    [Header("Egg Health")]
    public int maxEggs = 4;
    public int currentEggs = 4;
    public int hitsPerEgg = 3;
    public int damageTaken = 0;

    [Header("Motion Capture Preparation")]
    public GameObject mocapReadyPanel;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public TextMeshProUGUI resultText;

    private bool resetPending = false;
    private bool waitingForMocap = false;

    void Awake()
    {
        instance = this;

        currentEggs = maxEggs;
        damageTaken = 0;

        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        // Main menu stores Mouse = 0, Motion Capture = 2.
        int selectedMode = PlayerPrefs.GetInt(
            InputModePrefKey,
            0
        );

        waitingForMocap =
            selectedMode == (int)InputManager.InputMode.MocapLive;

        if (waitingForMocap)
        {
            // Freeze gameplay, but not Unity or OptiTrack.
            gameRunning = false;

            if (mocapReadyPanel != null)
                mocapReadyPanel.SetActive(true);

            Debug.Log(
                "DUCK KNIGHT: Waiting for knight to begin battle."
            );
        }
        else
        {
            gameRunning = true;

            if (mocapReadyPanel != null)
                mocapReadyPanel.SetActive(false);
        }
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }

    /// <summary>
    /// Called by Begin Battle button.
    /// Starts the round after mocap preparation.
    /// </summary>
    public void BeginBattle()
    {
        if (!waitingForMocap || gameRunning || resetPending)
            return;

        waitingForMocap = false;
        gameRunning = true;

        if (mocapReadyPanel != null)
            mocapReadyPanel.SetActive(false);

        Debug.Log(
            "DUCK KNIGHT: BEGIN, MY LIEGE! Battle commenced!"
        );
    }

    public void AddScore(int amount)
    {
        if (!gameRunning)
            return;

        score += amount;

        Debug.Log("DUCK KNIGHT: Score = " + score);
    }

    /// <summary>
    /// Applies damage to the current egg.
    /// Each egg breaks after hitsPerEgg hits.
    /// </summary>
    public bool TakeDamage()
    {
        if (!gameRunning || currentEggs <= 0)
            return false;

        damageTaken++;

        int safeHitsPerEgg = Mathf.Max(1, hitsPerEgg);

        if (AudioManager.instance != null)
        {
            if (damageTaken % safeHitsPerEgg == 0)
                AudioManager.instance.PlayEggBreakSound();
            else
                AudioManager.instance.PlayEggCrackSound();
        }

        currentEggs = Mathf.Max(
            0,
            maxEggs - (damageTaken / safeHitsPerEgg)
        );

        Debug.Log(
            "DUCK KNIGHT: Damage taken = " +
            damageTaken +
            " | Eggs remaining = " +
            currentEggs + " / " + maxEggs
        );

        if (currentEggs <= 0)
            LoseGame();

        return true;
    }

    private void WinGame()
    {
        if (!gameRunning)
            return;

        gameRunning = false;

        if (resultText != null)
            resultText.text = "THE KINGDOM IS SAFE!";

        if (endGamePanel != null)
            endGamePanel.SetActive(true);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayVictorySound();

        Debug.Log(
            "DUCK KNIGHT: VICTORY! Final Score: " + score
        );
    }

    private void LoseGame()
    {
        if (!gameRunning)
            return;

        gameRunning = false;

        if (resultText != null)
            resultText.text = "FAILURE!";

        if (endGamePanel != null)
            endGamePanel.SetActive(true);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayDefeatSound();

        Debug.Log(
            "DUCK KNIGHT: GAME OVER! All eggs broken. Final Score: " +
            score
        );
    }

    public void ResetGame()
    {
        if (resetPending)
            return;

        resetPending = true;
        gameRunning = false;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayResetSound();

        Invoke(nameof(ReloadCurrentScene), 0.65f);
    }

    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("DuckKnight_Menu");
    }
}
