using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game_Control
{

    //TODO: use _character_controller.isGrounded to check when player is airborne or not

    /// <summary>
    ///     State transition function for the player
    /// </summary>
    public class Player_State_Transition_Func : IState_Transition_Func
    {

        /*
   The player and the enemy are always in a state (pretty similar to animation states, but it contains other informaiton as well)
   */
        public enum player_state
        {
            idle,
            walking,
            running,
            jumping,
            attacking,
            dash_attack,
            jump_attack,
            dodging,
            attacked,
            parrying,
            parry_success,
            parry_fail
        }

        float dodge_duration = 0.25f;
        float parry_duration;

        int maximum_jump_count = 1;
        int jump_count = 0;
        float jump_time = 2f;//place holder jump_time, real jump_time should be based on fall speed



        //TODO: make a timer for cooldown
        float dodge_cooldown = 2.0f;
        float dodge_timer;
        float parry_cooldown = 1.0f;
        float parry_timer;

        CharacterController player_characterController;
        // CharacterController enemy_characterController;


        public Player_State_Transition_Func(CharacterController player){
            player_characterController = player;
        }


        /// <summary>
        /// helper to clean up code
        /// </summary>
        /// <param name="new_state"></param>
        /// <param name="new_duration"></param>
        /// <param name="curr_state"></param>
        /// <param name="prev_states"></param>
        /// <param name="duration"></param>
        private void update_state(int new_state, float new_duration, ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            prev_states.Add(curr_state);
            duration = new_duration;//attack is controlled by a seperate controller
            curr_state = new_state;
        }


        public void initialize(ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            curr_state = 0;
            duration = 0;
        }

        public bool process_state_with_player_input(ref int curr_state, ref List<int> prev_states, ref float duration, Player_Input.PlayerInput input)
        {

            if (curr_state == (int)player_state.jumping && player_characterController.isGrounded)
            {
                update_state((int)player_state.idle, 0, ref curr_state, ref prev_states, ref duration);
                return true;
            }

            if (curr_state == (int)player_state.dash_attack)
            {
                update_state((int)player_state.idle, 0, ref curr_state, ref prev_states, ref duration);
                return true;
            }

            //decrement dodge duration timer, or set state to idle if it has ended
            if (curr_state == (int)player_state.dodging)
            {
                if (duration <= 0)
                {
                    update_state((int)player_state.idle, 0, ref curr_state, ref prev_states, ref duration);
                    return true;
                }
                else
                {
                    duration -= Time.deltaTime;
                    return false;
                }
            }


            //an attack input has been issued
            //if we are already attacking, we won't change our state
            if (input.HasFlag(Player_Input.PlayerInput.Attack))
            {
                //if we aren't currently dodging/parrying/attacking, we will be moving into attack state
                if (curr_state != (int)player_state.parrying && 
                curr_state != (int)player_state.dodging && 
                curr_state != (int)player_state.attacking)
                {
                    if(curr_state == (int)player_state.jumping){
                        update_state((int)player_state.jump_attack, 0, ref curr_state, ref prev_states, ref duration);
                    }else 
                    if(curr_state == (int)player_state.running){
                        update_state((int)player_state.dash_attack, 0, ref curr_state, ref prev_states, ref duration);
                    }else{
                        update_state((int)player_state.attacking, 0, ref curr_state, ref prev_states, ref duration);
                    }
                    return true;
                }
                else
                {
                    //cannot make another attack, 
                    //or if it's in a combo in that case let the attack controller handle logic
                    return false;
                }
            }
            else


            // a jump input has been issued
            //process double jump if needed
            if (input.HasFlag(Player_Input.PlayerInput.Jump))
            {

                if (curr_state == (int)player_state.idle ||
                    curr_state == (int)player_state.walking ||
                    curr_state == (int)player_state.running)
                {
                    Debug.Log("curr_state : "+curr_state);

                    //start a new jump
                    jump_count = 1;
                    update_state((int)player_state.jumping, 0, ref curr_state, ref prev_states, ref duration);

                }
                else
                if (curr_state == (int)player_state.jumping && jump_count < 2)
                {
                    //another jump is allowed
                    prev_states.Add(curr_state);
                    jump_count++;
                    duration = 0;
                    Debug.Log("jump count : "+jump_count);
                }
                else
                {
                    //cannot update state
                    return false;
                }
                return true;
            }
            else

            //dodge or parry
            if (input.HasFlag(Player_Input.PlayerInput.Dodge))
            {

                if (curr_state == (int)player_state.idle ||
                    curr_state == (int)player_state.walking ||
                    curr_state == (int)player_state.running ||
                    curr_state == (int)player_state.jumping)
                {
                    //dodge!

                    //TODO: check for cooldown using a timer
                    update_state((int)player_state.dodging, dodge_duration, ref curr_state, ref prev_states, ref duration);
                    return true;
                }
                return false;
            }
            else

            //trying to parry
            if (input.HasFlag(Player_Input.PlayerInput.Parry))
            {

                if (curr_state == (int)player_state.idle ||
                    curr_state == (int)player_state.walking ||
                    curr_state == (int)player_state.running)
                {
                    //parry!

                    //TODO: check for cooldown using a timer
                    update_state((int)player_state.parrying, parry_duration, ref curr_state, ref prev_states, ref duration);
                    return true;
                }
                return false;
            }

            //trying to move
            if (input.HasFlag(Player_Input.PlayerInput.Dash) || input.HasFlag(Player_Input.PlayerInput.Walk))
            {

                if (curr_state == (int)player_state.idle ||
                    curr_state == (int)player_state.walking ||
                    curr_state == (int)player_state.running)
                {

                    //if not performing any special action, allow movement
                    int walk_or_run = (input.HasFlag(Player_Input.PlayerInput.Dash)) ? (int)player_state.running : (int)player_state.walking;
                    update_state(walk_or_run, 0, ref curr_state, ref prev_states, ref duration);
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool process_state(ref int curr_state, ref List<int> prev_states, ref float duration)
        {

            return false;
        }

    }

}