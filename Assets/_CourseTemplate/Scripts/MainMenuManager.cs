
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private const string InputModePrefKey = "LastInputMode";

    public void PlayMouseMode()
    {
        PlayerPrefs.SetInt(InputModePrefKey, 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("DuckKnight_Main");
    }

    public void PlayMocapMode()
    {
        PlayerPrefs.SetInt(InputModePrefKey, 2);
        PlayerPrefs.Save();

        SceneManager.LoadScene("DuckKnight_Main");
    }
}
