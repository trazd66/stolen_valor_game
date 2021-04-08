using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
   
    // void Start()
    // {
    //     startButton.Select();
    // }
    
    public void playhover()
    {
        AudioManager.instance.PlayOnce("buttonhover");
    }

    public void playpush()
    {
        AudioManager.instance.PlayOnce("buttonpress");
    }
}
