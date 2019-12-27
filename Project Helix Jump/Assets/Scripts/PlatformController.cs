using UnityEngine;

/// <summary>
/// Controlling rotation of all platforms.
/// </summary>
public class PlatformController : MonoBehaviour
{
    public float rotationSpeed = 50f;

    float xOldPos;
    float xPos;

    /// <summary>
    /// Check every frame if left mouse button is being held, and if it is then rotate all platforms in direction that mouse is being moved.
    /// </summary>
    void Update()
    {
        if (GameManager.instance.EOG || GameManager.instance.forceQuit || GameManager.instance.levelCleared)
            return;

        // save mouse position on click
        if (Input.GetMouseButtonDown(0))
            xOldPos = Input.mousePosition.x;

        // rotate platforms while draging mouse
        if (Input.GetMouseButton(0))
        {
            xPos = Input.mousePosition.x;
            transform.Rotate(0, (xOldPos - xPos) * rotationSpeed * Time.deltaTime, 0);

            xOldPos = xPos;
        }
    }
}
