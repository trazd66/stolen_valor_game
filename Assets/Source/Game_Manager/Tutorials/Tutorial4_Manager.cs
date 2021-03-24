using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Control;
using UnityEngine.UI;

public class Tutorial4_Manager : MonoBehaviour
{
    int tutorial_state = 0;

    public GameObject page0;

    public GameObject background;

    public GameObject tutorial_ui;

    public Player_controller player_controller;

    public Tutorial4_Reward reward;

    public Image dodge_indicator;

    private int dodge_counter;
    private bool dodging = false;

    public bool tutorial_finished = false;

    // Start is called before the first frame update
    void Start()
    {
        dodge_indicator.color = Color.red;

    }

    // Update is called once per frame
    void Update()
    {
        if (tutorial_state == 0 && Input.GetButtonDown("Jump"))
        {
            page0.SetActive(false);
            background.SetActive(false);
            tutorial_ui.SetActive(true);
            tutorial_state = 1;
        }
        else if (tutorial_state == 1)
        {
            player_controller.enable_control = true;

            Player_State_Transition_Func.player_state curr_state = player_controller.get_curr_state;

            if (curr_state == Player_State_Transition_Func.player_state.dodge)
            {
                dodging = true;
            }

            if (dodging && curr_state != Player_State_Transition_Func.player_state.dodge)
            {
                dodging = false;
                dodge_counter++;
            }

            if(dodge_counter >= 3)
            {
                dodge_indicator.color = Color.green;
                reward.placeReward(new Vector3(4, 2, -0.558f));
            }

        }

        if(tutorial_finished){
            Game_Manager.instance.setState(4);
        }
    }
}
