using UnityEngine;

/// <summary>
/// Class that is used for easier manipulation with sounds, it only contains information about specified sound.
/// </summary>
[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0, 1)]
    public float volume = 1;
    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}
