using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.pitch = s.pitch;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("sound " + name + " not found");
            return;
        }

        s.source.Play();
    }

    public void pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("sound " + name + " not found");
            return;
        }
        if (!s.source.isPlaying)
        {
            Debug.LogWarning("sound " + name + " not playing");
            return;
        }

        s.source.Pause();
    }

    public void stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("sound " + name + " not found");
            return;
        }
        if (!s.source.isPlaying)
        {
            Debug.LogWarning("sound " + name + " not playing");
            return;
        }

        s.source.Stop();
    }
}
