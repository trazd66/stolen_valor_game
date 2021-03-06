using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Util;

namespace Game_Control{

    public class Enemy1_controller : MonoBehaviour
    {
        State_controller state_controller;
        Enemy1_State_Transition_Func enemy1_state_transition_func;

        public HealthInfo player_health_info;
        public HealthInfo boss_health_info;
        private HealthBarApi ui;

        public GameObject player;

        public Collider[] hitboxes;

        public Animator enemy_animator;
        public Laser_Manager laser_manager;
        public Reward_Manager reward_manager;

        private float run_windup_speed = 1f;
        private float run_attack_speed = 5f;

        private float stomp_windup_speed = 2f;

        private float stomp_charge_horizontal_speed = 2.5f;
        private float stomp_charge_vertical_speed = 2.5f;
        private float stomp_attack_speed = 9.0f;


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
                //determine stomp horizontal speed
                else if(state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.stomp_charge)
                {
                    stomp_charge_horizontal_speed = 1.5f + Random.value;
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

            if((Enemy1_State_Transition_Func.enemy1_state) state_controller.curr_state == Enemy1_State_Transition_Func.enemy1_state.laser_charge){
                enemy_animator.Play(Utility_methods.GetDescription<Enemy1_State_Transition_Func.enemy1_state>((Enemy1_State_Transition_Func.enemy1_state)state_controller.curr_state));
            }else{
                string desc = Utility_methods.GetDescription<Enemy1_State_Transition_Func.enemy1_state>((Enemy1_State_Transition_Func.enemy1_state)state_controller.curr_state);
                if(desc != ""){
                    enemy_animator.Play(desc);
                }
            }

           
            if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.idle)
            {
                if (CheckPlayerBehind())
                {
                    transform.RotateAround(transform.position, Vector3.up, 180);
                }
            }
            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.run_windup)
            {
                transform.Translate(Time.deltaTime * -run_windup_speed, 0, 0);
            }
            //enemy is running to the right
            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_right)
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
            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.stomp_windup)
            {
                transform.Translate(Time.deltaTime * -stomp_windup_speed, 0, 0);
            }
            //enemy is charging stomp
            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.stomp_charge)
            {
                transform.Translate(Time.deltaTime * stomp_charge_horizontal_speed, Time.deltaTime * stomp_charge_vertical_speed, 0);
                if (transform.position.y >= 3f)
                {
                    transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
                }
                if (transform.position.x >= 4.5f && transform.right.x > 0)
                {
                    transform.position = new Vector3(4.5f, transform.position.y, transform.position.z);
                }
                if (transform.position.x <= -4.5f && transform.right.x < 0)
                {
                    transform.position = new Vector3(-4.5f, transform.position.y, transform.position.z);
                }

            }
            //enemy is stomping
            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.stomp_attack)
            {
                transform.Translate(0, Time.deltaTime * -stomp_attack_speed, 0);
                if (transform.position.y <= 0.66f)
                {
                    transform.position = new Vector3(transform.position.x, 0.66f, transform.position.z);
                }
            }

            LaunchAttack(hitboxes,(Enemy1_State_Transition_Func.enemy1_state)state_controller.curr_state);


        }

        private bool CheckPlayerBehind()
        {
            if ((transform.right.x >= 0 && player.transform.position.x < transform.position.x) ||
                 (transform.right.x < 0 && transform.position.x < player.transform.position.x))
            {
                return true;
            }
            return false;
        }

        //called when an attack is launched
        private bool LaunchAttack(Collider[] hitboxes, Enemy1_State_Transition_Func.enemy1_state state)
        {

            int damage = 0;

            foreach (Collider col in hitboxes)
            {
                Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("EnemyHitbox"));
                if (cols.Length > 0)
                {
                    switch (state)
                    {
                        case  Enemy1_State_Transition_Func.enemy1_state.run_attack_left:
                            damage = 30;
                            break;
                        case Enemy1_State_Transition_Func.enemy1_state.run_attack_right:
                            damage = 30;
                            break;
                        case Enemy1_State_Transition_Func.enemy1_state.stomp_attack:
                            damage = 50;
                            break;
                        default:
                            break;
                    }
                }
            }
            
            //return true if attack landed, false otherwise
            if (damage > 0)
            {
                if (player_health_info.parry_ready)
                {
                    player_health_info.setParrySuccess(true);
                    enemy1_state_transition_func.setJustHit();
                }
                else if (!player_health_info.is_invincible)
                {
                    player_health_info.doDamage(damage);
                    player_health_info.setInvincible(0.5f);
                    enemy1_state_transition_func.setJustHit();
                }
                


            }
            return false;
        }
    }
}
