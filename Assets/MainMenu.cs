using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Game_Control;

public class MainMenu : MonoBehaviour
{
    
    // void Start()
    // {
    //     startButton.Select();
    // }
    
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
