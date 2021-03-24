using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections.Generic;


public class AudioManager : MonoBehaviour
{

    Dictionary<string, AudioSource> sound_dic = new Dictionary<string, AudioSource>();
    public AudioClip[] clips;

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

        foreach (AudioClip ac in clips)
        {
            // for each audio clips in the clips we create a dictionary entry
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = ac;
            audioSource.loop = false;
            audioSource.volume = 1f;
            audioSource.playOnAwake = false;
            sound_dic.Add(ac.name,audioSource);
        }

    }

    void Start()
    {
        // SetLoop(Theme,true);
        // Play(Theme);
    }

    /// <summary>
    /// play a song
    /// </summary>
    /// <param name="name">song name</param>
    public void Play(string name)
    {
        if (sound_dic.ContainsKey(name))
        {
            sound_dic[name].Play();
        }

    }

    /// <summary>
    /// pause a song
    /// </summary>
    /// <param name="name">song name</param>
    public void Pause(string name)
    {
        if (sound_dic.ContainsKey(name))
        {
            if (sound_dic[name].isPlaying)
            {
                sound_dic[name].Play();
            }
            
        }
    }

    /// <summary>
    /// stop a song
    /// </summary>
    /// <param name="name">song name</param>
    public void Stop(string name)
    {
        if (sound_dic.ContainsKey(name))
        {
            if (sound_dic[name].isPlaying)
            {
                sound_dic[name].Stop();
            }

        }
    }

    /// <summary>
    /// change song volume
    /// </summary>
    /// <param name="name">song name</param>
    /// <param name="newVol">song volume</param>
    public void ChangeVolume(string name, float newVol)
    {
        if (sound_dic.ContainsKey(name))
        {
            sound_dic[name].volume = newVol;

        }
    }

    public void SetSFXVolume(float new_volume){
        foreach (string audio_name in sound_dic.Keys){
            if(!audio_name.Contains("leveltheme")){
                sound_dic[audio_name].volume = new_volume;
            }
        }
    }

    public void SetThemeVolume(float new_volume){
        foreach (string audio_name in sound_dic.Keys){
            if(audio_name.Contains("leveltheme")){
                sound_dic[audio_name].volume = new_volume;
            }
        }
    }


    /// <summary>
    /// change songs pitch
    /// </summary>
    /// <param name="name">song name</param>
    /// <param name="newPitch">song pitch</param>
    public void ChangePitch(string name, float newPitch)
    {
        if (sound_dic.ContainsKey(name))
        {
            sound_dic[name].pitch = newPitch;

        }
    }

    /// <summary>
    /// set a song is looping or not
    /// </summary>
    /// <param name="name">song name</param>
    /// <param name="looping">loop bool value, true for loop</param>
    public void SetLoop(string name, bool looping)
    {
        if (sound_dic.ContainsKey(name))
        {
            sound_dic[name].loop = looping;

        }
    }
}
