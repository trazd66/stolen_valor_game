using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game_Control
{
    public static class Player_controller_helper
    {

        public static Player_Input.PlayerInput getPlayerInput(ref ComboInfo combo_info, bool enable_attacks, bool enable_parry, bool enable_dodge, bool enable_laser)
        {

            Player_Input.PlayerInput input = Player_Input.PlayerInput.None;

            //check if player has inputed dash
            if (enable_dodge && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && Input.GetButtonDown("Dodge"))
            {
                input |= Player_Input.PlayerInput.Dodge;
            }
            //otherwise input parry
            else if (enable_parry && (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) && Input.GetButtonDown("Dodge"))
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
            if (enable_attacks && Input.GetButtonDown("Attack"))
            {
                input |= Player_Input.PlayerInput.Attack;
            }
            //check if laser is inputted
            if (enable_laser && Input.GetButtonDown("SpecialAttack1") && combo_info.canFireLaser())
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

    }

}
