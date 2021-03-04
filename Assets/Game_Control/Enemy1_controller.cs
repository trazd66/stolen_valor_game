using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game_Control{

    public class Enemy1_controller : MonoBehaviour
    {
        State_controller state_controller;
        Enemy1_State_Transition_Func enemy1_state_transition_func;

        float run_attack_speed = 5f;
        float front_attack_speed = 1.5f;


        public HealthInfo player_health_info;
        public HealthInfo boss_health_info;
        private HealthBarApi ui;

        public GameObject player;

        public GameObject[] AttackVisuals;
        public Renderer[] BossVisuals;


        public Collider[] AttackHitboxes;

        public Laser_Manager laser_manager;
        public Reward_Manager reward_manager;

        private float front_attack_range = 2.0f;


        // Start is called before the first frame update
        void Start()
        {
            state_controller = new State_controller();
            enemy1_state_transition_func = new Enemy1_State_Transition_Func(player, gameObject, boss_health_info);
            state_controller.initialize(enemy1_state_transition_func);

        }

        // Update is called once per frame
        void Update()
        {

            if (boss_health_info.is_dead)
            {
                reward_manager.placeReward(transform.position);
                Destroy(gameObject);
            }

            state_controller.process_time();

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
                //turn around if run attack collides with player
                if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_left &&
                    state_controller.prev_states[state_controller.prev_states.Count - 1] == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_right)
                {
                    transform.RotateAround(transform.position, Vector3.up, 180);
                }
                else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_right &&
                    state_controller.prev_states[state_controller.prev_states.Count - 1] == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_left)
                {
                    transform.RotateAround(transform.position, Vector3.up, 180);
                }
                //activate front attack hitboxes if front attack state has been entered
                else if(state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.front_attack)
                {
                    state_controller.state_duration = 0f;
                    state_controller.curr_state = (int)Enemy1_State_Transition_Func.enemy1_state.idle;
                }
                //remove front attack hitboxes whehn attack ends, and reset them to initial position
                else if(state_controller.prev_states[state_controller.prev_states.Count - 1] == (int)Enemy1_State_Transition_Func.enemy1_state.front_attack)
                {
                    AttackVisuals[2].transform.localPosition = new Vector3(1.5f, 0.9f, 0f);
                    AttackVisuals[2].SetActive(false);
                }
                else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_charge)
                {
                    if (transform.right.x >= 0)
                    {
                        laser_manager.aim_laser(transform.position, true);
                    }
                    if (transform.right.x < 0)
                    {
                        laser_manager.aim_laser(transform.position, false);
                    }
                }
                //call laser function if laser attack state is entered
                else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_attack)
                {
                    if (transform.right.x >= 0)
                    {
                        laser_manager.fire_laser(transform.position, true, true);
                    }
                    if (transform.right.x < 0)
                    {
                        laser_manager.fire_laser(transform.position, true, false);
                    }
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
            if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_charge)
            {
                for (int i = 0; i < BossVisuals.Length; i++)
                {
                    BossVisuals[i].material.SetColor("_Color", Color.black);
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
            

            //hitbox detecting for player touching enemy body
            LaunchAttack(AttackHitboxes[0]);
            LaunchAttack(AttackHitboxes[1]);

            //uncomment when you want to use the test attack
            //LaunchAttack(AttackHitboxes[4]);


        }



        //called when an attack is launched
        private bool LaunchAttack(Collider col)
        {


            //check what Colliders on the PlayerHitbox layer overlap col
            Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("PlayerHitbox"));

            int damage = 0;

            if (cols.Length > 0)
            {

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
            }
            
            //return true if attack landed, false otherwise
            if (damage > 0 && !player_health_info.is_invincible)
            {
                if (player_health_info.parry_ready)
                {
                    player_health_info.setParrySuccess(true);
                }
                else
                {
                    player_health_info.doDamage(damage);
                    player_health_info.setInvincible(0.5f);
                }
                enemy1_state_transition_func.setJustHit();


            }
            return false;
        }
    }
}
