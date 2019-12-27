using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlls UI text object behaviour.
/// </summary>
public class ScoreTextController : MonoBehaviour
{
    public int point = 1;
    public float upSpeed = 100;

    Text t;
    float size = 30;

    /// <summary>
    /// Define text content and time when it will be destroyed.
    /// </summary>
    void Start()
    {
        t = GetComponent<Text>();
        t.text = "+" + point;
        
        Destroy(gameObject, .5f);
    }

    /// <summary>
    /// Every frame move text upwards and size it up.
    /// </summary>
    private void Update()
    {
        size += 40 * Time.deltaTime;
        t.fontSize = (int)size;
        transform.position += new Vector3(0, upSpeed * Time.deltaTime, 0);
    }
}
