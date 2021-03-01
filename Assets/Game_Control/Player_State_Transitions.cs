using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Util;
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
            walk,
            dash,
            jump,
            airborne,
            attack_basic,
            attack_dash,
            attack_jump,
            attack_special_0,
            dodge,
            parry_active,
            parry_cooldown,
            attacked
        }


        int maximum_jump_count = 2;
        int jump_count = 0;

        float parry_active_duration = 0.1f;
        float parry_cooldown_duration = 0.5f;

        float dash_attack_duration = 0.9f;
        float jump_attack_duration = 0.3f;

        float dodge_duration = 0.1f;
        float dodge_cooldown = 1.0f;
        public Float_ref dodge_cd_timer;

        float laser_cooldown = 2.0f;
        public Float_ref laser_cd_timer;

        CharacterController player_characterController;
        // CharacterController enemy_characterController;


        public Player_State_Transition_Func(CharacterController player)
        {
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


        public void initialize(ref int curr_state, ref List<int> prev_states, ref float duration, ref List<Float_ref> cooldown_timers)
        {
            curr_state = 0;
            duration = 0;
            dodge_cd_timer = new Float_ref(dodge_cooldown);
            cooldown_timers.Add(dodge_cd_timer);
            laser_cd_timer = new Float_ref(laser_cooldown);
            cooldown_timers.Add(laser_cd_timer);
        }

        public bool process_state_with_player_input(ref int curr_state, ref List<int> prev_states, ref float duration, Player_Input.PlayerInput input)
        {
            if(player_characterController.isGrounded && curr_state == (int)player_state.attack_jump)
            {
                update_state((int)player_state.idle, 0, ref curr_state, ref prev_states, ref duration);
            }
            if(duration <= 0){
                if (curr_state == (int)player_state.parry_active)
                {
                    update_state((int)player_state.parry_cooldown, parry_cooldown_duration, ref curr_state, ref prev_states, ref duration);
                }

                else if (player_characterController.isGrounded &&  curr_state != (int)player_state.idle)
                {
                    update_state((int)player_state.idle, 0, ref curr_state, ref prev_states, ref duration);
                }
                else if (!player_characterController.isGrounded &&  curr_state != (int)player_state.airborne)
                {
                    update_state((int)player_state.airborne, 0, ref curr_state, ref prev_states, ref duration);
                }

            }else{
                return false;
            }

            //if multiple inputs are present, attack has the highest priority
            if (input.HasFlag(Player_Input.PlayerInput.Attack))
            {
                //if we aren't currently dodge/parrying/attack_basic, we will be moving into attack state
                if (curr_state != (int)player_state.dodge &&
                    curr_state != (int)player_state.attack_basic &&
                    curr_state != (int)player_state.attack_jump &&
                    curr_state != (int)player_state.attack_dash &&
                    curr_state != (int)player_state.attack_special_0)
                {
                    if((curr_state == (int)player_state.idle ||
                    curr_state == (int)player_state.walk ||
                    curr_state == (int)player_state.dash ||
                    curr_state == (int)player_state.airborne) && input.HasFlag(Player_Input.PlayerInput.Special_attack_0))
                    {
                        //only shoot laser if cooldown is expired
                        if(laser_cd_timer.Value <= 0) {
                            update_state((int)player_state.attack_special_0, 0, ref curr_state, ref prev_states, ref duration);
                            laser_cd_timer.Value = laser_cooldown;
                        }
                        else
                        //do nothing if cooldown isn't expired
                        {
                            return false;
                        }
                        

                    }
                    else
                    if (curr_state == (int)player_state.airborne || input.HasFlag(Player_Input.PlayerInput.Jump))
                    {
                        update_state((int)player_state.attack_jump, jump_attack_duration, ref curr_state, ref prev_states, ref duration);
                    }
                    else
                    if (curr_state == (int)player_state.dash || input.HasFlag(Player_Input.PlayerInput.Dash))
                    {
                        update_state((int)player_state.attack_dash, dash_attack_duration, ref curr_state, ref prev_states, ref duration);
                    }
                    else
                    if (curr_state == (int)player_state.idle)
                    {
                        update_state((int)player_state.attack_basic, 0, ref curr_state, ref prev_states, ref duration);
                    }

                    return true;
                }
                // else do nothing
                return false;
            }
            else
            // process parry input
            if (input.HasFlag(Player_Input.PlayerInput.Parry))
            {
                if (curr_state == (int)player_state.idle ||
                    curr_state == (int)player_state.walk ||
                    curr_state == (int)player_state.dash)
                {
                    update_state((int)player_state.parry_active, parry_active_duration, ref curr_state, ref prev_states, ref duration);
                    return true;
                }
            }
            else
            if (input.HasFlag(Player_Input.PlayerInput.Jump))
            {

                if (curr_state == (int)player_state.idle ||
                    curr_state == (int)player_state.walk ||
                    curr_state == (int)player_state.dash)
                {
                    //start a new jump
                    jump_count = 1;
                    update_state((int)player_state.jump, 0, ref curr_state, ref prev_states, ref duration);

                }
                else
                if (curr_state == (int)player_state.airborne && jump_count < maximum_jump_count)
                {
                    //another jump is allowed
                    update_state((int)player_state.jump, 0, ref curr_state, ref prev_states, ref duration);
                    jump_count++;
                }
                else
                {
                    //cannot update state
                    return false;
                }
                return true;
            }
            else
            if (input.HasFlag(Player_Input.PlayerInput.Dodge) && dodge_cd_timer.Value <= 0)
            {

                if (curr_state == (int)player_state.idle ||
                    curr_state == (int)player_state.walk ||
                    curr_state == (int)player_state.dash ||
                    curr_state == (int)player_state.airborne)
                {
                    update_state((int)player_state.dodge, dodge_duration, ref curr_state, ref prev_states, ref duration);
                    dodge_cd_timer.Value = dodge_cooldown;
                    return true;
                }
                return false;
            }
            else
            //trying to move
            if (input.HasFlag(Player_Input.PlayerInput.Dash) || input.HasFlag(Player_Input.PlayerInput.Walk))
            {

                if (curr_state == (int)player_state.idle ||
                    curr_state == (int)player_state.walk ||
                    curr_state == (int)player_state.dash)
                {

                    //if not performing any special action, allow movement
                    int walk_or_dash = (input.HasFlag(Player_Input.PlayerInput.Dash)) ? (int)player_state.dash : (int)player_state.walk;
                    update_state(walk_or_dash, 0, ref curr_state, ref prev_states, ref duration);
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool process_state(ref int curr_state, ref List<int> prev_states, ref float duration)
        {

            if(duration <= 0){
                if (player_characterController.isGrounded &&  curr_state != (int)player_state.idle)
                {
                    update_state((int)player_state.idle, 0, ref curr_state, ref prev_states, ref duration);
                }
                else if (!player_characterController.isGrounded &&  curr_state != (int)player_state.airborne)
                {
                    update_state((int)player_state.airborne, 0, ref curr_state, ref prev_states, ref duration);
                }

            }
            return false;
        }
    }

}