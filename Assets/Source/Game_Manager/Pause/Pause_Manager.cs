using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause_Manager : MonoBehaviour
{
    public GameObject pause_screen;
    public GameObject control_screen;
    public GameObject laser_tutorial;
    public GameObject combo_tutorial;

    private bool paused = false;
    private bool laser_paused = false;
    private bool combo_paused = false;

    public void ShowPause()
    {
        pause_screen.SetActive(true);
    }

    public void RemovePause()
    {
        pause_screen.SetActive(false);
    }

    public void ShowControls()
    {
        control_screen.SetActive(true);
    }

    public void RemoveControls()
    {
        control_screen.SetActive(false);
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

    public void ShowComboTutorial()
    {
        combo_tutorial.SetActive(true);
    }

    public void RemoveComboTutorial()
    {
        combo_tutorial.SetActive(false);
    }

    public void PauseCombo()
    {
        combo_paused = true;
    }

    public void UnpauseCombo()
    {
        combo_paused = false;
    }

    public bool GetComboPaused()
    {
        return combo_paused;
    }



}

