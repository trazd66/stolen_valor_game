using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    public void playhover()
    {
        AudioManager.instance.PlayOnce("buttonhover");
    }

    public void playpush()
    {
        AudioManager.instance.PlayOnce("buttonpress");
    }
}
