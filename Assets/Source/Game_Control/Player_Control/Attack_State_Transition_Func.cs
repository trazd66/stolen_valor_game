using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Util;
using System.ComponentModel;
namespace Game_Control
{

    public class Attack_State_Transition_Func : IState_Transition_Func
    {
        public enum attack_state
        {
            not_attacking,
            [Description("attack_basic_0")]
            attack_basic_0,
            [Description("attack_basic_1")]
            attack_basic_1,
            [Description("attack_basic_2")]
            attack_basic_2,
            [Description("attack_basic_3")]
            attack_basic_3,

            [Description("attack_dash_0")]
            attack_dash_0,
            [Description("attack_jump_0")]
            attack_jump_0,
            attack_special_0,

        }


        private float basic_attack_interval = 0.5f;
        private float strong_attack_interval = 0.8f;
        private float jump_attack_interval = 0.5f;
        private float dash_attack_interval = 0.4f;
        private float special_attack_interval_0 = 0.5f;

        private Queue<int> attack_queue;

        private void update_state(int new_state, float new_duration, ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            prev_states.Add(curr_state);
            duration = new_duration;
            curr_state = new_state;
        }

        public void initialize(ref int curr_state, ref List<int> prev_states, ref float duration, ref List<Float_ref> cooldown_timers)
        {
            curr_state = 0;
            duration = 0.0f;
            attack_queue = new Queue<int>(8);
        }

        public void improve_attack_interval(float factor){
            basic_attack_interval /= factor;
            strong_attack_interval /= factor;
            jump_attack_interval /= factor;
            dash_attack_interval /= factor;
            special_attack_interval_0 /= factor;
        }



        //dequeue from attack queue and proceed with state machine
        public bool process_state(ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            if (duration <= 0)
            {
                //finished previous attack

                if (attack_queue.Count > 0)
                {
                    int atk = attack_queue.Dequeue();

                    //basic attack
                    if (atk == (int)Player_Input.PlayerInput.Attack)
                    {
                        if (curr_state < (int)attack_state.attack_basic_2)
                        {
                            update_state(curr_state + 1, basic_attack_interval, ref curr_state, ref prev_states, ref duration);
                        }
                        else if (curr_state == (int)attack_state.attack_basic_2)
                        {
                            update_state(curr_state + 1, strong_attack_interval, ref curr_state, ref prev_states, ref duration);
                        }else{
                            update_state((int)attack_state.attack_basic_0, basic_attack_interval, ref curr_state, ref prev_states, ref duration);
                        }

                    }
                    else
                    //special attack 0
                    if (atk == (int)(Player_Input.PlayerInput.Attack | Player_Input.PlayerInput.Special_attack_0))
                    {
                        update_state((int)attack_state.attack_special_0, special_attack_interval_0, ref curr_state, ref prev_states, ref duration);
                    }
                    //dash attack
                    else
                    if (atk == (int)(Player_Input.PlayerInput.Dash | Player_Input.PlayerInput.Attack))
                    {
                        if (curr_state == (int)attack_state.not_attacking)
                        {
                            AudioManager.instance.Play("char_spinattk");
                            update_state((int)attack_state.attack_dash_0, dash_attack_interval, ref curr_state, ref prev_states, ref duration);
                        }

                    }
                    else
                    //jump attack
                    if (atk == (int)(Player_Input.PlayerInput.Jump | Player_Input.PlayerInput.Attack))
                    {
                        //AudioManager.instance.Play("game jam 3 impact");
                        if (curr_state < (int)attack_state.attack_basic_3)
                        {
                            update_state((int)attack_state.attack_jump_0, jump_attack_interval, ref curr_state, ref prev_states, ref duration);
                        }
                        else if (curr_state < (int)attack_state.attack_jump_0)
                        {
                            update_state(curr_state + 1, jump_attack_interval, ref curr_state, ref prev_states, ref duration);
                        }

                    }
                    return true;
                }
                else if (curr_state != (int)attack_state.not_attacking)
                {
                    update_state((int)attack_state.not_attacking, 0.0f, ref curr_state, ref prev_states, ref duration);
                    return true;
                }


            }
            return false;
        }

        //add input to queue, that's it
        public bool process_state_with_player_input(ref int curr_state, ref List<int> prev_states, ref float duration, Player_Input.PlayerInput input)
        {

            //enqueue attack if attack queue is not full
            if (input.HasFlag(Player_Input.PlayerInput.Attack) && attack_queue.Count <= 8)
            {
                attack_queue.Enqueue((int)input);
                return true;
            }
            //clear queue if non-attack action is inputted, and store what action that is
            else
            if (!(input == Player_Input.PlayerInput.None) && attack_queue.Count > 0)
            {
                attack_queue.Clear();
            }


            return false;
        }
    }
}
