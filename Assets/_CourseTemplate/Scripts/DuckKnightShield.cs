
using UnityEngine;

public class DuckKnightShield : MonoBehaviour
{
    [Header("References")]
    public InputManager inputManager;
    public GameObject shieldVisual;

    void Start()
    {
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    void Update()
    {
        if (inputManager == null || shieldVisual == null)
            return;

        // Show the shield only during its activation window.
        bool shouldBeActive = inputManager.ShieldActive;

        if (shieldVisual.activeSelf != shouldBeActive)
        {
            shieldVisual.SetActive(shouldBeActive);
        }
    }
}
