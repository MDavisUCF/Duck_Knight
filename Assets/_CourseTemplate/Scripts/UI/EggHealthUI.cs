
using UnityEngine;
using UnityEngine.UI;

public class EggHealthUI : MonoBehaviour
{
    [Header("Egg Images")]
    public Image[] eggImages;

    [Header("Egg Sprites")]
    public Sprite wholeEgg;
    public Sprite crackedEgg;
    public Sprite[] brokenEggs;

    void Update()
    {
        if (GameManager.instance == null)
            return;

        GameManager game = GameManager.instance;

        int hitsPerEgg = Mathf.Max(1, game.hitsPerEgg);

        for (int i = 0; i < eggImages.Length; i++)
        {
            if (eggImages[i] == null)
                continue;

            int hitsOnThisEgg = Mathf.Clamp(
                game.damageTaken - (i * hitsPerEgg),
                0,
                hitsPerEgg
            );

            if (hitsOnThisEgg >= hitsPerEgg)
            {
                if (brokenEggs != null &&
                    brokenEggs.Length > 0)
                {
                    eggImages[i].sprite =
                        brokenEggs[i % brokenEggs.Length];
                }
            }
            else if (hitsOnThisEgg >= 1)
            {
                eggImages[i].sprite = crackedEgg;
            }
            else
            {
                eggImages[i].sprite = wholeEgg;
            }
        }
    }
}
