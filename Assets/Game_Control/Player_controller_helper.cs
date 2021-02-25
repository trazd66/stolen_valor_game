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
            if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && Input.GetMouseButtonDown(1))
            {
                input |= Player_Input.PlayerInput.Dodge;
            }
            //otherwise check if left or right are inputed
            else if (Input.GetAxis("Horizontal") != 0)
            {
                input |= Player_Input.PlayerInput.Dash;
            }

            //check if jump is inputed
            if (Input.GetButtonDown("Jump"))
            {
                input |= Player_Input.PlayerInput.Jump;
            }

            //check if attack is inputted
            if (Input.GetMouseButtonDown(0))
            {
                input |= Player_Input.PlayerInput.Attack;
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

        public static void do_attack(Attack_State_Transition_Func.attack_state attack_state, Renderer[] visuals, Collider[] hitboxes, HealthInfo boss_health_info){
            Debug.Log("ATTACK");

            Collider col  = hitboxes[0];
            visuals[0].enabled = true;
                 //check what Colliders on the PlayerHitbox layer overlap col
            Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("EnemyHitbox"));
            if(cols.Length > 0){
                Debug.Log("hit");
                //cols[0].gameObject.GetComponentInParent<HealthInfo>().curr_health -= 10;
                boss_health_info.doDamage(50);
                boss_health_info.setInvincible(0.3f);
            }

        }



    }

}
