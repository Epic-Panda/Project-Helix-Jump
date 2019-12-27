using UnityEngine;

/// <summary>
/// Controlls platform pieces motion. Used for destroy platform pieces.
/// </summary>
public class MovingPlatformController : MonoBehaviour
{
    public float startAngle;
    public float endAngle;

    public float movingSpeed = 15;
    float angle;

    float yRotation = 0;

    /// <summary>
    /// Define starting values of variables used to rotate platform pieces.
    /// </summary>
    private void Start()
    {
        angle = movingSpeed;

        startAngle = (startAngle + 360) % 360;
        endAngle = (endAngle + 360) % 360;

        if (endAngle < startAngle)
        {
            float temp = endAngle;
            endAngle = startAngle;
            startAngle = temp;
        }

        transform.localRotation = Quaternion.Euler(0, startAngle, 0);
    }

    /// <summary>
    /// Every frame determinate if direction of rotation is changed, then execute rotation.
    /// </summary>
    void Update()
    {
        yRotation = transform.localRotation.eulerAngles.y;

        if (yRotation >= endAngle)
            angle = -movingSpeed;
        else if (yRotation <= startAngle)
            angle = movingSpeed;

        transform.Rotate(0, angle * Time.deltaTime, 0);
    }
}
