using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause_Manager : MonoBehaviour
{
    public GameObject pause_screen;
    public GameObject laser_tutorial;

    private bool paused = false;
    private bool laser_paused = false;

    public void ShowPause()
    {
        pause_screen.SetActive(true);
    }

    public void RemovePause()
    {
        pause_screen.SetActive(false);
    }

    public void PauseGame()
    {
        paused = true;
    }

    public void UnpauseGame()
    {
        paused = false;
    }

    public bool GetPaused()
    {
        return paused;
    }

    public void ShowLaserTutorial()
    {
        laser_tutorial.SetActive(true);
    }

    public void RemoveLaserTutorial()
    {
        laser_tutorial.SetActive(false);
    }

    public void PauseLaser()
    {
        laser_paused = true;
    }

    public void UnpauseLaser()
    {
        laser_paused = false;
    }

    public bool GetLaserPaused()
    {
        return laser_paused;
    }


}

