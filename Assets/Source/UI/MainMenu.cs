using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Game_Control;

public class MainMenu : MonoBehaviour
{

    public Slider theme_volume;
    public Slider sfx_volume;

    public AudioManager audio_manager;

    void Awake()
    {
        audio_manager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        theme_volume.onValueChanged.AddListener(delegate { audio_manager.SetThemeVolume(theme_volume.value); });
        sfx_volume.onValueChanged.AddListener(delegate { audio_manager.SetSFXVolume(sfx_volume.value); });
    }

    public void PlayGame()
    {
        Game_Manager.instance.setState(4);
    }

    public void PlayTutorial()
    {
        Game_Manager.instance.setState(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void playhover()
    {
       AudioManager.instance.PlayOnce("buttonhover");
    }

    public void playpush()
    {
        AudioManager.instance.PlayOnce("buttonpress");
    }

}
