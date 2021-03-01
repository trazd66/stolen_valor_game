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

            //Does the same thing as above
            float normalize = Math.Max(Math.Abs(horizontal), Math.Abs(vertical));
            return new Vector3(horizontal / normalize, vertical / normalize, 0);
        }

        public static void do_attack(Attack_State_Transition_Func.attack_state attack_state, Collider[] hitboxes, HealthInfo boss_health_info)
        {

            int damage = 0;

            foreach (Collider col in hitboxes)
            {
                Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("EnemyHitbox"));
                if (cols.Length > 0)
                {
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
                //hit detected
                break;
            }
            //check what Colliders on the PlayerHitbox layer overlap col

            if (damage > 0)
            {
                boss_health_info.doDamage(damage);
                boss_health_info.setInvincible(0.4f);
            }

        }



    }

}
