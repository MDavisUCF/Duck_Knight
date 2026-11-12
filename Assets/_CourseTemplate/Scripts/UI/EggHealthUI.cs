
using TMPro;
using UnityEngine;

public class EggHealthUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text eggHealthText;

    [Header("Placeholder Characters")]
    public string wholeEgg = "●";
    public string brokenEgg = "✕";

    void Update()
    {
        if (GameManager.instance == null || eggHealthText == null)
            return;

        int currentEggs = GameManager.instance.currentEggs;
        int maxEggs = GameManager.instance.maxEggs;

        string display = "";

        for (int i = 0; i < maxEggs; i++)
        {
            display += i < currentEggs ? wholeEgg : brokenEgg;

            if (i < maxEggs - 1)
                display += " ";
        }

        eggHealthText.text = display;
    }
}
