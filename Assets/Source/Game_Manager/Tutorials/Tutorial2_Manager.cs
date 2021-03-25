using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Control;
using UnityEngine.UI;

public class Tutorial2_Manager : MonoBehaviour
{

    int tutorial_state = 0;

    public GameObject page0;

    public GameObject background;

    public GameObject tutorial_ui;

    public Player_controller player_controller;

    private bool attacked = false;
    private bool dash_attacked = false;
    private bool jump_attacked = false;

    public Tutorial2_Reward reward;

    public Image attack_indicator;
    public Image dash_attack_indicator;
    public Image jump_attack_indicator;

    public bool tutorial_finished = false;

    void Start()
    {
        attack_indicator.color = Color.red;
        dash_attack_indicator.color = Color.red;
        jump_attack_indicator.color = Color.red;
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
        else if(tutorial_state == 1)
        {
            player_controller.enable_control = true;

            Attack_State_Transition_Func.attack_state curr_state = player_controller.curr_atk_state;

            if(curr_state == Attack_State_Transition_Func.attack_state.attack_basic_4)
            {
                attacked = true;
                attack_indicator.color = Color.green;
            }
            else if (curr_state == Attack_State_Transition_Func.attack_state.attack_dash_0)
            {
                dash_attacked = true;
                dash_attack_indicator.color = Color.green;
            }
            else if (curr_state == Attack_State_Transition_Func.attack_state.attack_jump_0)
            {
                jump_attacked = true;
                jump_attack_indicator.color = Color.green;
            }

            if(attacked && dash_attacked && jump_attacked)
            {
                reward.placeReward(new Vector3(4, 2, -0.558f));
            }
        }

        if(tutorial_finished){
            Game_Manager.instance.setState(2);
        }
    }
}
