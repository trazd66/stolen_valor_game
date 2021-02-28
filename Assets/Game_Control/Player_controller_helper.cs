using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game_Control
{
    public static class Player_controller_helper
    {

        public static Player_Input.PlayerInput getPlayerInput()
        {

            Player_Input.PlayerInput input = Player_Input.PlayerInput.None;

            //check if player has inputed dash
            if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && Input.GetButtonDown("Dodge"))
            {
                input |= Player_Input.PlayerInput.Dodge;
            }
            else if (Input.GetButtonDown("Dodge"))
            {
                input |= Player_Input.PlayerInput.Parry;
            }
            //otherwise check if left or right are inputed
            else if (Math.Abs(Input.GetAxis("Horizontal")) > 0.9)
            {
                input |= Player_Input.PlayerInput.Dash;
            }

            //check if jump is inputed
            if (Input.GetButtonDown("Jump"))
            {
                input |= Player_Input.PlayerInput.Jump;
            }

            //check if attack is inputted
            if (Input.GetButtonDown("Attack"))
            {
                input |= Player_Input.PlayerInput.Attack;
            }

            if (Input.GetButtonDown("SpecialAttack1"))
            //if (Input.GetKeyDown("i"))
            {
                input |= Player_Input.PlayerInput.Attack;
                input |= Player_Input.PlayerInput.Special_attack_0;
            }

            return input;

        }

        public static Vector3 getDodgeVector(float horizontal, float vertical)
        {

            //             if (state_changed && state_controller.curr_state == (int)Player_State_Transition_Func.player_state.dodging)
            // {
            //     dodge_horizontal = Math.Abs(Input.GetAxis("Horizontal"));
            //     dodge_vertical = Math.Abs(Input.GetAxis("Vertical"));

            //     ui.set_player_invincible(0.2f);

            //     if(dodge_vertical > dodge_horizontal)
            //     {
            //         dodge_horizontal = dodge_horizontal * 1 / dodge_vertical;
            //         dodge_vertical = 1;
            //     }
            //     else
            //     {
            //         dodge_vertical = dodge_vertical * 1 / dodge_horizontal;
            //         dodge_horizontal = 1;
            //     }

            //     if (Input.GetAxis("Horizontal") < 0)
            //     {
            //         dodge_horizontal = dodge_horizontal * -1;
            //     }
            //     if (Input.GetAxis("Vertical") < 0)
            //     {
            //         dodge_vertical = dodge_vertical * -1;
            //     }

            // }
            //Does the same thing as above
            float normalize = Math.Max(Math.Abs(horizontal),Math.Abs(vertical));
            return new Vector3(horizontal/normalize,vertical/normalize,0);
        }

        public static void do_attack(Attack_State_Transition_Func.attack_state attack_state, Renderer[] visuals, Collider[] hitboxes, ref float combo_counter, ref float combo_timer, 
            HealthInfo player_health_info, HealthInfo boss_health_info){
            Debug.Log("ATTACK");

            //default is basic attack
            Collider col = hitboxes[0];
            Renderer vis = visuals[0];

            //select hurtbox based on state
            if (attack_state == Attack_State_Transition_Func.attack_state.attack_basic_0 ||
                attack_state == Attack_State_Transition_Func.attack_state.attack_basic_1 ||
                attack_state == Attack_State_Transition_Func.attack_state.attack_basic_2 ||
                attack_state == Attack_State_Transition_Func.attack_state.attack_basic_3 ||
                attack_state == Attack_State_Transition_Func.attack_state.attack_basic_4)
            {
                col = hitboxes[0];
                vis = visuals[0];
            }
            else
            if (attack_state == Attack_State_Transition_Func.attack_state.attack_dash_0){
                col = hitboxes[5];
                vis = visuals[5];
            }
            else
            if (attack_state == Attack_State_Transition_Func.attack_state.attack_dash_1)
            {
                col = hitboxes[6];
                vis = visuals[6];
            }
            else
            if (attack_state == Attack_State_Transition_Func.attack_state.attack_dash_2)
            {
                col = hitboxes[7];
                vis = visuals[7];
            }

            vis.enabled = true;

            int damage = 0;
            int combo_points;
                 
            //check what Colliders on the PlayerHitbox layer overlap col
            Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("EnemyHitbox"));
            if(cols.Length > 0){
                Debug.Log("hit");
                switch (col.name)
                {
                    case "BasicAttack":
                        damage += 50;
                        break;
                    case "DashAttack1":
                        damage += 30;
                        break;
                    case "DashAttack2":
                        damage += 70;
                        break;
                    case "DashAttack3":
                        damage += 70;
                        break;
                    case "JumpAttack":
                        damage += 40;
                        break;
                    default:
                        Debug.Log("Unable to identify attack, make sure switch case matches.");
                        break;
                }
            }
            if (damage > 0)
            {
                boss_health_info.doDamage(damage);
                boss_health_info.setInvincible(0.4f);
                combo_counter++;
                combo_timer = 1.5f;
                combo_points = damage;
                float combo_multiplier = 1.0f + (combo_counter * 0.1f);
                if (combo_multiplier > 2.0f)
                {
                    combo_multiplier = 2.0f;
                }
                combo_points = (int)(combo_points * combo_multiplier);
                if (player_health_info.parry_bonus)
                {
                    combo_points = (int)(combo_points * 1.5f);
                }
                Debug.Log(combo_counter);
                Debug.Log(combo_points);
            }

        }



    }

}
