using UnityEngine;

/// <summary>
/// Controlling camera behaviour.
/// </summary>
public class CameraController : MonoBehaviour
{
    public Vector2 yConstraint = new Vector2(-32,37);
    public GameObject target;
    public Vector3 offset;

    [Header("Camera settings")]
    public float speed = 10f;
    
    /// <summary>
    /// Updates position of camera every frame.
    /// </summary>
    void Update()
    {
        if (GameManager.instance.EOG || GameManager.instance.levelCleared || GameManager.instance.forceQuit)
            return;

        if (!target)
            return;

        SmoothMotionY(target.transform.position);
    }

    /// <summary>
    /// Moving camera to determinated position.
    /// </summary>
    /// <param name="destination">Destionation is position that camera is going to.</param>
    void SmoothMotionY(Vector3 destination)
    {
        destination += offset;
        destination.y = Mathf.Clamp(destination.y, yConstraint.x, yConstraint.y);
        destination.x = transform.position.x;
        destination.z = transform.position.z;

        Vector3 smoothPosition = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);

        transform.position = smoothPosition;
    }
}
