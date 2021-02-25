using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game_Control{

    public class Enemy1_controller : MonoBehaviour
    {
        State_controller state_controller;

        float run_attack_speed = 5f;
        float front_attack_speed = 1.5f;
        float laser_attack_speed = 8f;

        public HealthInfo player_health_info;
        public HealthInfo boss_health_info;
        private HealthBarApi ui;

        public GameObject[] AttackVisuals;
        public Renderer[] BossVisuals;


        public Collider[] AttackHitboxes;


        // Start is called before the first frame update
        void Start()
        {
            state_controller = new State_controller();
            state_controller.initialize(new Enemy1_State_Transition_Func());

        }

        // Update is called once per frame
        void Update()
        {
            bool state_changed = false;
            state_changed = state_controller.process_state();

            //if state just changed
            if (state_changed)
            {   
                //rotate if idle state was just entered from run attack state
                if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.idle && 
                    state_controller.prev_states[state_controller.prev_states.Count - 1] == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_right)
                {
                    transform.RotateAround(transform.position, Vector3.up, 180);
                }
                else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.idle &&
                    state_controller.prev_states[state_controller.prev_states.Count - 1] == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_left)
                {
                    transform.RotateAround(transform.position, Vector3.up, 180);
                }
                //activate front attack hitboxes if front attack state has been entered
                else if(state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.front_attack)
                {
                    AttackVisuals[2].SetActive(true);
                }
                //remove front attack hitboxes whehn attack ends, and reset them to initial position
                else if(state_controller.prev_states[state_controller.prev_states.Count - 1] == (int)Enemy1_State_Transition_Func.enemy1_state.front_attack)
                {
                    AttackVisuals[2].transform.localPosition = new Vector3(1.5f, 0.9f, 0f);
                    AttackVisuals[2].SetActive(false);
                }
                //activate laser attack hitboxes if front attack state has been entered
                else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_attack)
                {
                    AttackVisuals[3].SetActive(true);
                }
                //remove laser attack hitboxes when attack ends, and reset them to initial position
                else if (state_controller.prev_states[state_controller.prev_states.Count - 1] == (int)Enemy1_State_Transition_Func.enemy1_state.laser_attack)
                {
                    AttackVisuals[3].transform.localPosition = new Vector3(0.7f, 0.75f, 0f);
                    AttackVisuals[3].SetActive(false);
                }
            }

            //apply colour
            if (boss_health_info.is_invincible)
            {
                for (int i = 0; i < BossVisuals.Length; i++)
                {
                    BossVisuals[i].material.SetColor("_Color", Color.red);
                }
            }
            else
            {
                for (int i = 0; i < BossVisuals.Length; i++)
                {
                    BossVisuals[i].material.SetColor("_Color", Color.gray);
                }
            }


            //enemy is running to the right
            if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_right)
            {
                transform.Translate(Time.deltaTime * run_attack_speed, 0, 0);
                if (transform.position.x >= 4)
                {
                    transform.position = new Vector3(4f, transform.position.y, transform.position.z);
                }
            }
            //enemy is running to the left
            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_left)
            {
                transform.Translate(Time.deltaTime * run_attack_speed, 0, 0);
                if (transform.position.x <= -4)
                {
                    transform.position = new Vector3(-4f, transform.position.y, transform.position.z);                  
                }
            }
            //enemy is front attacking
            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.front_attack)
            {
                LaunchAttack(AttackHitboxes[2]);
                AttackVisuals[2].transform.Translate(0, -Time.deltaTime * front_attack_speed, 0);
            }
            //enemy is laser attacking
            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_attack)
            {
                LaunchAttack(AttackHitboxes[3]);
                AttackVisuals[3].transform.Translate(Time.deltaTime * laser_attack_speed, 0, 0);
            }

            //hitbox detecting for player touching enemy body
            LaunchAttack(AttackHitboxes[0]);
            LaunchAttack(AttackHitboxes[1]);

            //uncomment when you want to use the test attack
            //LaunchAttack(AttackHitboxes[4]);


        }





        //called when an attack is launched
        private void LaunchAttack(Collider col)
        {


            //check what Colliders on the PlayerHitbox layer overlap col
            Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("PlayerHitbox"));

            int final_damage = 0;

            foreach (Collider c in cols)
            {
                int damage = 0;

                //add damage based on what's attacking
                switch (col.name)
                {
                    case "Body":
                        damage += 20;
                        break;
                    case "Head":
                        damage += 20;
                        break;
                    case "Front":
                        damage += 80;
                        break;
                    case "Laser":
                        damage += 50;
                        break;
                    case "TestDamage":
                        damage += 10;
                        break;
                    default:
                        Debug.Log("Unable to identify attack, make sure switch case matches.");
                        break;
                }

                //add damage based on what's being hit
                switch (c.name)
                {
                    case "Body":
                        damage += 10;
                        break;
                    case "Eyes":
                        damage += 10;
                        break;
                    default:
                        Debug.Log("Unable to identify target, make sure switch case matches.");
                        break;
                }

                //TODO some kind of priority system for when an attack hits multiple hitboxes at the same time, but you only want to apply damage once
                final_damage = damage;
            }
            
            //return true if attack landed, false otherwise
            if (final_damage > 0)
            {
                player_health_info.doDamage(final_damage);
                player_health_info.setInvincible(0.5f);
            }
        }
    }
}
