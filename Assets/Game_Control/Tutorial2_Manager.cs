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

    public Player_controller player_Controller;

    private bool attacked = false;
    private bool dash_attacked = false;
    private bool jump_attacked = false;

    public Tutorial2_Reward reward;

    public Image attack_indicator;
    public Image dash_attack_indicator;
    public Image jump_attack_indicator;

    // Start is called before the first frame update
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
            player_Controller.enable_control = true;

            Player_State_Transition_Func.player_state curr_state = player_Controller.get_curr_state;

            if(curr_state == Player_State_Transition_Func.player_state.attack_basic)
            {
                attacked = true;
                attack_indicator.color = Color.green;
            }
            else if (curr_state == Player_State_Transition_Func.player_state.attack_dash)
            {
                dash_attacked = true;
                dash_attack_indicator.color = Color.green;
            }
            else if (curr_state == Player_State_Transition_Func.player_state.attack_jump)
            {
                jump_attacked = true;
                jump_attack_indicator.color = Color.green;
            }

            if(attacked && dash_attacked && jump_attacked)
            {
                reward.placeReward(new Vector3(4, 2, -0.558f));
            }
        }
    }
}
