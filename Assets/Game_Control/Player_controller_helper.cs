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

            float normalize = Math.Max(Math.Abs(horizontal), Math.Abs(vertical));
            return new Vector3(horizontal / normalize, vertical / normalize, 0);
        }


       public static void do_attack(Attack_State_Transition_Func.attack_state attack_state, Collider[] hitboxes, ComboInfo combo_info, 
            HealthInfo player_health_info, HealthInfo boss_health_info){
            int damage = 0;

            foreach (Collider col in hitboxes)
            {
                Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("EnemyHitbox"));
                if (cols.Length > 0)
                {
                    switch (attack_state)
                    {
                        case Attack_State_Transition_Func.attack_state.attack_basic_0:
                            damage = 30;
                            break;
                        case Attack_State_Transition_Func.attack_state.attack_basic_1:
                            damage = 30;
                            break;
                        case Attack_State_Transition_Func.attack_state.attack_basic_2:
                            damage = 50;
                            break;
                        case Attack_State_Transition_Func.attack_state.attack_basic_3:
                            damage = 50;
                            break;
                        case Attack_State_Transition_Func.attack_state.attack_basic_4:
                            damage = 50;
                            break;
                        case Attack_State_Transition_Func.attack_state.attack_dash_0:
                            damage = 40;
                            break;
                        case Attack_State_Transition_Func.attack_state.attack_jump_0:
                            damage = 40;
                            break;
                        default:
                            Debug.Log("Unable to identify attack, make sure switch case matches.");
                            break;
                    }
                }
            }

            int cur_combo_points;                 
            if (damage > 0 && !boss_health_info.is_invincible)
            {

                if (player_health_info.parry_bonus)
                {
                    damage = (int)(damage * 1.5);
                }

                boss_health_info.doDamage(damage);
                Debug.Log(damage);
                AudioManager.instance.Play("game jam 3 impact");

                boss_health_info.setInvincible(0.4f);
                combo_info.setComboTimer(1.5f);
                    
                cur_combo_points = damage;                   
                float combo_multiplier = 1.0f + (combo_info.combo_counter * 0.1f);    
                
                if (combo_multiplier > 2.0f)                    
                {                        
                    combo_multiplier = 2.0f;                    
                }                    
                cur_combo_points = (int)(cur_combo_points * combo_multiplier);             
                
                combo_info.addComboPoints(cur_combo_points);
                combo_info.combo_counter++;
                
            }

        }



    }

}
