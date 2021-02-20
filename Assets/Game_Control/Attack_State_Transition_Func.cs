using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game_Control
{

    public class Attack_State_Transition_Func : IState_Transition_Func
    {
        public enum attack_state
        {
            not_attacking,
            attack_basic_0,
            attack_basic_1,            
            attack_basic_2, 
            attack_basic_3,
            attack_basic_4,
            attack_jump_0,
            attack_dash_0,
            attack_special_0,
        }


        private float basic_attack_interval = 0.8f;
        private float jump_attack_interval = 0.8f;
        private float dash_attack_interval = 0.8f;

        private Queue<int> attack_queue;

        private void update_state(int new_state, float new_duration, ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            prev_states.Add(curr_state);
            duration = new_duration;
            curr_state = new_state;
        }

        public void initialize(ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            curr_state = 0;
            duration = 3.0f;
            attack_queue = new Queue<int>(200);
        }

        

        //dequeue from attack queue and proceed with state machine
        public bool process_state(ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            duration -= Time.deltaTime;
            if(duration <= 0){
                //finished previous attack

                if (attack_queue.Count > 0){
                    int atk = attack_queue.Dequeue();
                    //basic attack
                    if(atk == (int)Player_Input.PlayerInput.Attack){
                        if(curr_state < 5){
                            curr_state++;
                            duration = basic_attack_interval;
                        }else 
                        if (curr_state == 5){
                            curr_state = 1;
                            duration = basic_attack_interval;
                        }
                    }else 
                    //dash attack
                    if(atk == (int) (Player_Input.PlayerInput.Dash | Player_Input.PlayerInput.Attack)){
                        if(curr_state < 5){
                            curr_state = (int)attack_state.attack_dash_0;
                            duration = dash_attack_interval;
                        }

                    }else
                    //jump attack
                    if(atk == (int) (Player_Input.PlayerInput.Dash | Player_Input.PlayerInput.Attack)){
                        if(curr_state < 5){
                            curr_state = (int)attack_state.attack_jump_0;
                            duration = jump_attack_interval;
                        }

                    }

                }else{
                    curr_state = 0;
                    duration = 0;
                }

            }
            return false;
        }

        //add input to queue, that's it
        public bool process_state_with_player_input(ref int curr_state, ref List<int> prev_states, ref float duration, Player_Input.PlayerInput input)
        {
            if(input.HasFlag(Player_Input.PlayerInput.Attack)){
                attack_queue.Enqueue((int)input);
                return true;
            }else 
            if (!(input == Player_Input.PlayerInput.None)){
                attack_queue.Clear();
            }
            return false;
        }
    }
}