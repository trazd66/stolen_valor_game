using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Util;
namespace Game_Control
{

    public class Enemy1_State_Transition_Func : IState_Transition_Func
    {
        public enum enemy1_state
        {
            idle,
            run_attack_right,
            run_attack_left,
            front_attack,
            laser_attack
        }

        int direction_facing = 1; //0 for facing left, 1 for facing right

        private void update_state(int new_state, float new_duration, ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            prev_states.Add(curr_state);
            duration = new_duration;
            curr_state = new_state;
        }

        public void initialize(ref int curr_state, ref List<int> prev_states, ref float duration, ref List<Float_ref> cooldown_timers)
        {
            curr_state = 0;
            duration = 3.0f;
        }

        

        public bool process_state(ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            bool state_changed = false;
            //boss is idle and has not attacked in three seconds
            if (curr_state == (int)enemy1_state.idle && duration <= 0)
            {
                float rand = Random.value * 3;

                //perform run attack in appropriate direction
                if (rand < 1)
                {
                    if (direction_facing == 0)
                    {
                        update_state((int)enemy1_state.run_attack_left, 1.6f, ref curr_state, ref prev_states, ref duration);
                    }
                    else
                    {
                        update_state((int)enemy1_state.run_attack_right, 1.6f, ref curr_state, ref prev_states, ref duration);
                    }
                }
                //perform front attack
                else if (rand < 2)
                {
                    update_state((int)enemy1_state.front_attack, 1.07f, ref curr_state, ref prev_states, ref duration);
                    //Debug.Log("state changed 3");
                }
                //perform laser attack
                else
                {
                    update_state((int)enemy1_state.laser_attack, 1.25f, ref curr_state, ref prev_states, ref duration);
                    //Debug.Log("state changed 4");
                }
                state_changed = true;


            }
            //boss has completed run attack to the right
            else if (curr_state == (int)enemy1_state.run_attack_right && duration <= 0)
            {
                update_state((int)enemy1_state.idle, 3.0f, ref curr_state, ref prev_states, ref duration);
                //Debug.Log("state changed 0");
                direction_facing = 0;
                state_changed = true;
            }
            //boss has completed run attack to the left
            else if (curr_state == (int)enemy1_state.run_attack_left && duration <= 0)
            {
                update_state((int)enemy1_state.idle, 3.0f, ref curr_state, ref prev_states, ref duration);
                //Debug.Log("state changed 0");
                direction_facing = 1;
                state_changed = true;
            }
            //boss has completed front attack
            else if (curr_state == (int)enemy1_state.front_attack && duration <= 0)
            {
                update_state((int)enemy1_state.idle, 3.0f, ref curr_state, ref prev_states, ref duration);
                //Debug.Log("state changed 0");
                state_changed = true;
            }
            //boss has completed laser attack
            else if (curr_state == (int)enemy1_state.laser_attack && duration <= 0)
            {
                update_state((int)enemy1_state.idle, 3.0f, ref curr_state, ref prev_states, ref duration);
                //Debug.Log("state changed 0");
                state_changed = true;
            }
            else
            {
                duration -= Time.deltaTime;
            }

            return state_changed;
        }

        public bool process_state_with_player_input(ref int curr_state, ref List<int> prev_states, ref float duration, Player_Input.PlayerInput input)
        {
            return false;
        }
    }
}
