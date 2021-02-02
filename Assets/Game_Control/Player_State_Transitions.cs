using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game_Control
{

    //TODO: use _character_controller.isGrounded to check when player is airborne or not

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
        dodging,
        attacked,
        parrying,
        parry_success,
        parry_fail
    }



    /// <summary>
    ///     State transition function for the player
    /// </summary>
    public class Player_State_Transition_Func : IState_Transition_Func
    {

        float dodge_duration;
        float parry_duration;

        int maximum_jump_count = 1;
        int jump_count = 0;
        float jump_time = 2f;//place holder jump_time, real jump_time should be based on fall speed



        //TODO: make a timer for cooldown
        float dodge_cooldown = 2.0f;
        float dodge_timer;
        float parry_cooldown = 1.0f;
        float parry_timer;


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

            //an attack input has been issued
            //if we are already attacking, we won't change our state
            if (input.HasFlag(Player_Input.PlayerInput.Attack))
            {
                //if we aren't currently dodging/parrying/attacking, we will be moving into attack state
                if (curr_state != (int)player_state.parrying || curr_state != (int)player_state.dodging || curr_state != (int)player_state.attacking)
                {
                    update_state((int)player_state.attacking, 0, ref curr_state, ref prev_states, ref duration);
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

                if (curr_state != (int)player_state.parrying ||
                    curr_state != (int)player_state.dodging ||
                    curr_state != (int)player_state.attacking)
                {

                    //start a new jump
                    jump_count = 1;
                    update_state((int)player_state.jumping, jump_time, ref curr_state, ref prev_states, ref duration);

                }
                else
                if (curr_state == (int)player_state.jumping && jump_count < maximum_jump_count)
                {
                    //another jump is allowed
                    prev_states.Add(curr_state);
                    jump_count++;
                    duration += jump_time;
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

                if (curr_state != (int)player_state.parrying ||
                    curr_state != (int)player_state.dodging ||
                    curr_state != (int)player_state.attacking)
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

                if (curr_state != (int)player_state.parrying ||
                    curr_state != (int)player_state.dodging ||
                    curr_state != (int)player_state.attacking)
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

                if (curr_state != (int)player_state.parrying ||
                    curr_state != (int)player_state.dodging ||
                    curr_state != (int)player_state.attacking ||
                    curr_state != (int)player_state.jumping)
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

            //set all states back to idle once their duration ends
            if (curr_state == (int)player_state.parrying ||
                    curr_state == (int)player_state.dodging ||
                    curr_state == (int)player_state.attacking ||
                    curr_state == (int)player_state.jumping)
            {
                if (duration <= 0)
                {
                    update_state((int)player_state.idle, 0, ref curr_state, ref prev_states, ref duration);
                    return true;
                }
            }

            return false;
        }

    }

}