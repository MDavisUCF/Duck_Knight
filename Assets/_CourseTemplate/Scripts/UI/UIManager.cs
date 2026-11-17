
using TMPro;
using UnityEngine;

/// <summary>
/// Updates Duck Knight's gameplay HUD.
/// Displays score, timer, shield status,
/// input mode, controls, and round results.
/// Egg health is handled by EggHealthUI.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("HUD Text References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI propStateText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI inputModeText;
    public TextMeshProUGUI resetText;
    public TextMeshProUGUI instructionsText;

    [Header("Scene References")]
    public InputManager inputManager;

    void Update()
    {
        if (GameManager.instance == null)
            return;

        GameManager game = GameManager.instance;

        bool isMocapLive =
            inputManager != null &&
            inputManager.currentMode == InputManager.InputMode.MocapLive;

        // SCORE
        if (scoreText != null)
        {
            scoreText.text = "Score: " + game.score;
        }

       
        // TIMER
        if (timerText != null)
        {
            int remainingSeconds = Mathf.Max(
                0,
                Mathf.CeilToInt(game.roundTime)
            );

            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;

            timerText.text = $"{minutes}:{seconds:00}";
        }


        // SHIELD STATUS
        if (propStateText != null)
        {
            if (!game.gameRunning)
            {
                propStateText.text = "Shield: INACTIVE";
            }
            else if (inputManager != null &&
                     inputManager.ShieldActive)
            {
                propStateText.text =
                    "Shield: ACTIVE (" +
                    inputManager.ShieldTimeRemaining.ToString("F1") +
                    "s)";
            }
            else if (isMocapLive)
            {
                propStateText.text = "Shield: NOT TRACKED";
            }
            else
            {
                propStateText.text = "Shield: READY";
            }
        }

        // INPUT MODE
        if (inputModeText != null)
        {
            inputModeText.text = isMocapLive
                ? "Mode: Motion Capture"
                : "Mode: Debug Keyboard";
        }

        // INSTRUCTIONS / ROUND RESULT
        if (instructionsText != null)
        {
            if (!game.gameRunning)
            {
                instructionsText.text =
                    game.currentEggs <= 0
                        ? "GAME OVER! All eggs broken!"
                        : "VICTORY! The pond is safe!";
            }
            else if (isMocapLive)
            {
                instructionsText.text =
                    "Sword: Move & Attack\n" +
                    "Shield: Block incoming threats";
            }
            else
            {
                instructionsText.text =
                    "WASD: Move  |  SPACE: Shield";
            }
        }

        // RESTART / MODE SWITCH
        if (resetText != null)
        {
            resetText.text =
                "M: Switch Mode  |  R: Restart";
        }
    }
}
