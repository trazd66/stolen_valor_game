using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Util;
using System.ComponentModel;


namespace Game_Control
{

    public class Enemy1_State_Transition_Func : IState_Transition_Func
    {
        public enum enemy1_state
        {
            [Description("idle")]
            idle,
            run_windup,
            run_attack_right,
            run_attack_left,
            front_attack,
            [Description("laser_charge")]
            laser_charge,
            laser_attack,
            stomp_windup,
            stomp_charge,
            stomp_attack
        }

        private HealthInfo boss_health_info;
        private GameObject player;
        private GameObject enemy1;
        private bool just_hit = false;
        private bool just_stomped = false;

        private float base_idle_duration = 3.0f;
        private float hurt_idle_duration = 1.5f;

        private float stomp_attack_range = 2.5f;


        public Enemy1_State_Transition_Func(GameObject player_ref, GameObject enemy1_ref, HealthInfo boss_health_info_ref)
        {
            boss_health_info = boss_health_info_ref;
            player = player_ref;
            enemy1 = enemy1_ref;
        }

        private void update_state(int new_state, float new_duration, ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            prev_states.Add(curr_state);
            duration = new_duration;
            curr_state = new_state;
        }

        public void initialize(ref int curr_state, ref List<int> prev_states, ref float duration, ref List<Float_ref> cooldown_timers)
        {
            curr_state = 0;
            duration = 3.0f;
        }

        

        public bool process_state(ref int curr_state, ref List<int> prev_states, ref float duration)
        {
            bool state_changed = false;
            bool just_hit_temp = false;
            bool just_stomped_temp = false;

            float idle_duration = base_idle_duration;

            //ensure just_hit gets reset to false after this function is called, but store it's current value if true.
            if (just_hit)
            {
                just_hit_temp = true;
                just_hit = false;
            }

            if (just_stomped)
            {
                just_stomped_temp = true;
                just_stomped = false;
            }

            if (boss_health_info.curr_health / boss_health_info.max_health <= 0.5f)
            {
                idle_duration = hurt_idle_duration;
            }

            //turn around if doing run attack and collided with player
            if (curr_state == (int)enemy1_state.run_attack_right && just_hit_temp)
            {
                update_state((int)enemy1_state.run_attack_left, 0, ref curr_state, ref prev_states, ref duration);
                return true;
            }

            if (curr_state == (int)enemy1_state.run_attack_left && just_hit_temp)
            {
                update_state((int)enemy1_state.run_attack_right, 0, ref curr_state, ref prev_states, ref duration);
                return true;
            }

            //activate recharge stomp if it connected
            if (curr_state == (int)enemy1_state.stomp_attack && just_stomped_temp)
            {
                update_state((int)enemy1_state.stomp_charge, 0, ref curr_state, ref prev_states, ref duration);
                return true;
            }

            //go back to idle after stomp attack
            if (curr_state == (int)enemy1_state.stomp_attack && enemy1.transform.position.y == 0.66f)
            {
                update_state((int)enemy1_state.idle, idle_duration, ref curr_state, ref prev_states, ref duration);
                return true;
            }

            //boss is idle and has not attacked in three seconds
            if (curr_state == (int)enemy1_state.idle && duration <= 0)
            {
                float rand = -1;

                List<int> invalid_rolls = new List<int>();

                if (!PlayerIsWithinRange(stomp_attack_range))
                {
                    invalid_rolls.Add(1);
                }
                if (CheckPlayerBehind())
                {
                    invalid_rolls.Add(0);
                }
                bool valid = false;
                while (!valid)
                {
                    valid = true;
                    rand = Random.value * 3;
                    foreach(int roll in invalid_rolls)
                    {
                        if((int)rand == roll){
                            valid = false;
                            break;
                        }
                    }
                }

                //perform run attack in appropriate direction
                if (rand < 1)
                {
                    update_state((int)enemy1_state.run_windup, 0.5f, ref curr_state, ref prev_states, ref duration);
                }
                //perform stomp attack
                else if (rand < 2)
                {
                    update_state((int)enemy1_state.stomp_windup, 0.2f, ref curr_state, ref prev_states, ref duration);
                    //Debug.Log("state changed 3");
                }
                //perform laser attack
                else
                {
                    update_state((int)enemy1_state.laser_charge, 1.25f, ref curr_state, ref prev_states, ref duration);
                    //Debug.Log("state changed 4");
                }
                state_changed = true;


            }
            //boss has completed run windup
            else if (curr_state == (int)enemy1_state.run_windup && duration <= 0)
            {
                if (enemy1.transform.right.x < 0)
                {
                    update_state((int)enemy1_state.run_attack_left, 0f, ref curr_state, ref prev_states, ref duration);
                }
                else
                {
                    update_state((int)enemy1_state.run_attack_right, 0f, ref curr_state, ref prev_states, ref duration);
                }
            }
            
            //boss has completed run attack to the right
            else if (curr_state == (int)enemy1_state.run_attack_right && enemy1.transform.position.x == 4f)
            {
                update_state((int)enemy1_state.idle, idle_duration, ref curr_state, ref prev_states, ref duration);
                //Debug.Log("state changed 0");
                state_changed = true;
            }
            //boss has completed run attack to the left
            else if (curr_state == (int)enemy1_state.run_attack_left && enemy1.transform.position.x == -4f)
            {
                update_state((int)enemy1_state.idle, idle_duration, ref curr_state, ref prev_states, ref duration);
                //Debug.Log("state changed 0");
                state_changed = true;
            }
            //boss has completed front attack
            else if (curr_state == (int)enemy1_state.front_attack && duration <= 0)
            {
                update_state((int)enemy1_state.idle, idle_duration, ref curr_state, ref prev_states, ref duration);
                //Debug.Log("state changed 0");
                state_changed = true;
            }
            //boss has completed laser charge
            else if (curr_state == (int)enemy1_state.laser_charge && duration <= 0)
            {
                update_state((int)enemy1_state.laser_attack, 0.5f, ref curr_state, ref prev_states, ref duration);
                //Debug.Log("state changed 0");
                state_changed = true;
            }
            //boss has completed laser attack
            else if (curr_state == (int)enemy1_state.laser_attack && duration <= 0)
            {
                update_state((int)enemy1_state.idle, idle_duration, ref curr_state, ref prev_states, ref duration);
                //Debug.Log("state changed 0");
                state_changed = true;
            }
            else if (curr_state == (int)enemy1_state.stomp_windup && duration <= 0)
            {
                update_state((int)enemy1_state.stomp_charge, 0f, ref curr_state, ref prev_states, ref duration);
                state_changed = true;
            }
            //activate stomp if peak stomp charge height has been reached
            else if (curr_state == (int)enemy1_state.stomp_charge && enemy1.transform.position.y == 3f)
            {
                update_state((int)enemy1_state.stomp_attack, 0, ref curr_state, ref prev_states, ref duration);
                state_changed = true;
            }
            //go back to idle after stomp attack
            else if (curr_state == (int)enemy1_state.stomp_attack && enemy1.transform.position.y == 0.66f)
            {
                update_state((int)enemy1_state.idle, idle_duration, ref curr_state, ref prev_states, ref duration);
                state_changed = true;
            }

            return state_changed;
        }

        public bool process_state_with_player_input(ref int curr_state, ref List<int> prev_states, ref float duration, Player_Input.PlayerInput input)
        {
            return false;
        }

        //return true if player is within range units in front of the boss
        private bool PlayerIsWithinRange(float range)
        {
            if ((enemy1.transform.right.x >= 0 && player.transform.position.x - enemy1.transform.position.x <= range) ||
                 (enemy1.transform.right.x < 0 && enemy1.transform.position.x - player.transform.position.x <= range))
            {
                return true;
            }

            return false;
        }

        private bool CheckPlayerBehind()
        {
            if ((enemy1.transform.position.x < -3.75 && player.transform.position.x < enemy1.transform.position.x) ||
                 (enemy1.transform.position.x > 3.75 && enemy1.transform.position.x < player.transform.position.x))
            {
                return true;
            }
            return false;
        }


        public void setJustHit()
        {
            just_hit = true;
        }

        public void setJustStomped()
        {
            just_stomped = true; 
        }
    }
}
