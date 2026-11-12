
using UnityEngine;

public class DuckKnightMovement : MonoBehaviour
{
    [Header("Sword Tracking")]
    public Transform swordTransform;

    [Header("Duck Movement")]
    public float groundHeight = 0f;
    public Vector3 positionOffset = Vector3.zero;

    void LateUpdate()
    {
        if (swordTransform == null)
            return;

        // Follow the sword on the horizontal plane.
        // Ignore the sword's height and rotation.
        transform.position = new Vector3(
            swordTransform.position.x + positionOffset.x,
            groundHeight,
            swordTransform.position.z + positionOffset.z
        );
    }
}
