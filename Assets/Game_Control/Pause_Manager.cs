using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause_Manager : MonoBehaviour
{
    public Image pause_image;
    public Text pause_text;

    public void ShowPause()
    {
        pause_image.enabled = true;
        pause_text.enabled = true;
    }

    public void RemovePause()
    {
        pause_image.enabled = false;
        pause_text.enabled = false;
    }
}

