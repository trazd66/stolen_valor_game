using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game_Control
{
    public static class Player_controller_helper
    {

        public static Player_Input.PlayerInput getPlayerInput(ref ComboInfo combo_info)
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
            //check if laser is inputted
            if (Input.GetButtonDown("SpecialAttack1") && combo_info.canFireLaser())
            {
                input |= Player_Input.PlayerInput.Attack;
                input |= Player_Input.PlayerInput.Special_attack_0;
            }

            return input;

        }

        public static Vector3 getDodgeVector(float horizontal, float vertical)
        {
            //normalize the direction vector to preserve input direction
            float normalize = Math.Max(Math.Abs(horizontal),Math.Abs(vertical));
            return new Vector3(horizontal/normalize,vertical/normalize,0);
        }

        public static void do_attack(Attack_State_Transition_Func.attack_state attack_state, Renderer[] visuals, Collider[] hitboxes, ComboInfo combo_info, 
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
            else
            if (attack_state == Attack_State_Transition_Func.attack_state.attack_jump_0)
            {
                col = hitboxes[8];
                vis = visuals[8];
            }
            else
            if (attack_state == Attack_State_Transition_Func.attack_state.attack_jump_1)
            {
                col = hitboxes[9];
                vis = visuals[9];
            }
            else
            if (attack_state == Attack_State_Transition_Func.attack_state.attack_jump_2)
            {
                col = hitboxes[10];
                vis = visuals[10];
            }

            vis.enabled = true;

            int damage = 0;
            int cur_combo_points;
                 
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
                        damage += 20;
                        break;
                    case "DashAttack2":
                        damage += 60;
                        break;
                    case "DashAttack3":
                        damage += 60;
                        break;
                    case "JumpAttack1":
                        damage += 30;
                        break;
                    case "JumpAttack2":
                        damage += 30;
                        break;
                    case "JumpAttack3":
                        damage += 30;
                        break;
                    default:
                        Debug.Log("Unable to identify attack, make sure switch case matches.");
                        break;
                }
            }
            if (damage > 0)
            {
                boss_health_info.doDamage(damage);
                if (!boss_health_info.is_invincible)
                {
                    boss_health_info.setInvincible(0.4f);
                    combo_info.setComboTimer(1.5f);
                    cur_combo_points = damage;
                    float combo_multiplier = 1.0f + (combo_info.combo_counter * 0.1f);
                    if (combo_multiplier > 2.0f)
                    {
                        combo_multiplier = 2.0f;
                    }
                    cur_combo_points = (int)(cur_combo_points * combo_multiplier);
                    if (player_health_info.parry_bonus)
                    {
                        cur_combo_points = (int)(combo_info.getComboPoints() * 1.5f);
                    }
                    combo_info.addComboPoints(cur_combo_points);
                    combo_info.combo_counter++;
                }
                
            }

        }



    }

}
