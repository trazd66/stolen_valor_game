using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Control;

public class Tutorial1_Manager : MonoBehaviour
{

    int tutorial_state = 0;

    public GameObject page0;
    public GameObject page1;

    public GameObject background;

    public GameObject tutorial_ui;

    public Player_controller player_Controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorial_state == 0 && Input.GetButtonDown("Jump"))
        {
            page0.SetActive(false);
            page1.SetActive(true);
            tutorial_state = 1;
        }

        else if (tutorial_state == 1 && Input.GetButtonDown("Jump"))
        {
            page1.SetActive(false);
            background.SetActive(false);
            tutorial_ui.SetActive(true);
            tutorial_state = 2;
        }


        else if (tutorial_state == 1 && Input.GetButtonDown("Attack"))
        {
            page1.SetActive(false);
            page0.SetActive(true);
            tutorial_state = 0;
        }


        else if(tutorial_state == 2)
        {
            player_Controller.enable_control = true;
        }

    }
}
