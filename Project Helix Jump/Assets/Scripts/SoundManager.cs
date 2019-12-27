using UnityEngine;
using UnityEngine.Audio;
using System;

/// <summary>
/// Class that is used for sound managment.
/// Contain all sounds.
/// Class is singleton.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] sounds;

    bool mute = false;

    /// <summary>
    /// First function to be called, makes sure there is no any other sound manager, and defines all sounds using Sound.class.
    /// </summary>
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    /// <summary>
    /// Plays specified sound.
    /// </summary>
    /// <param name="name">Sound name.</param>
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s != null)
            s.source.Play();
    }

    /// <summary>
    /// Turn on/off all sounds.
    /// </summary>
    public void ToggleSound()
    {
        mute = !mute;

        foreach (Sound s in sounds)
            if (mute)
                s.source.volume = 0;
            else
                s.source.volume = s.volume;
    }
}
