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
        public Laser_Reward_Manager laser_reward_manager;
        public Reward_Manager reward_manager;


        private float lerpSpeed;

        private Quaternion rot;
        private int phase = 1;

        private float run_windup_speed = 2f;
        private float run_attack_speed = 7.5f;

        private float stomp_windup_speed = 2f;

        private float stomp_charge_horizontal_speed = 2.5f;
        private float stomp_charge_vertical_speed = 2.5f;
        private float stomp_attack_speed = 9.0f;

        private Vector3[] laser_rain_positions = new Vector3[10];
        private Vector3[] laser_rain_directions = new Vector3[10];

        private Vector3 laser_player_pos;

        private Vector3 laser_rapid_position;
        private Vector3 laser_rapid_direction;

        public Enemy1_State_Transition_Func get_enemy1_state_transition_func
        {
            get
            {
                return enemy1_state_transition_func;
            }
        }

        public Enemy1_State_Transition_Func.enemy1_state curr_state
        {
            get
            {
                return (Enemy1_State_Transition_Func.enemy1_state)state_controller.curr_state;
            }
        }

        public bool get_knockback_direction()
        {
            return transform.right.x >= 0;
        }

        // Start is called before the first frame update
        void Start()
        {
            state_controller = new State_controller();
            enemy1_state_transition_func = new Enemy1_State_Transition_Func(player, gameObject, boss_health_info);
            state_controller.initialize(enemy1_state_transition_func);

        }

        public IEnumerator delayed_pulses()

        {
            var pos = transform.position;
            pos.y -= 2f;
            Particle_system_controller.Instance.set_particle(CONTROL_CONFIG.VFX_SHOCK_WAVE,pos,0.5f);
            yield return new WaitForSeconds(0.05f);
            StopCoroutine(delayed_pulses());
        }

        public void slerp_rotate(){
            lerpSpeed = 20 * Time.deltaTime;
                rot = Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, lerpSpeed);
        }

        public IEnumerator fire_laser_with_delay(Vector3 position, bool is_enemy, Vector3 direction, float radius, float max_distance, float delay)
        {
            yield return new WaitForSeconds(delay);
            laser_manager.fire_laser(position,is_enemy,direction,radius,max_distance);
            StopCoroutine(fire_laser_with_delay(laser_rapid_position, is_enemy, laser_rapid_direction, 0.5f, 30f,delay));        }

        public IEnumerator aim_laser_with_delay(Vector3 position, Vector3 direction, float radius, float max_distance, float delay)
        {
            yield return new WaitForSeconds(delay);
            laser_manager.aim_laser(position,direction,radius,max_distance);
            StopCoroutine(aim_laser_with_delay(laser_rapid_position, laser_rapid_direction, 0.5f, 30f,delay));
        }

        public void laser_consecutive_fire(bool is_enemy, float radius, float max_distance, int num_laser_fired, float delay_in_between){
            for (int i = 0; i < num_laser_fired; i++)
            {
                float rand_x = (Random.value * 20) - 10;

                laser_rapid_position = new Vector3(rand_x, 10f, -0.558f);
                laser_rapid_direction = player.transform.position;

                StartCoroutine(aim_laser_with_delay(laser_rapid_position, laser_rapid_direction, 0.5f, 30f,delay_in_between*i));
                StartCoroutine(fire_laser_with_delay(laser_rapid_position, is_enemy, laser_rapid_direction, 0.5f, 30f,delay_in_between*i + 1.25f));
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (boss_health_info.is_dead)
            {
                reward_manager.placeReward(transform.position);
                Destroy(gameObject);
            }

            if(boss_health_info.curr_health / boss_health_info.max_health <= 0.5f && phase == 1)
            {
                phase = 2;
                laser_reward_manager.placeReward();
                StartCoroutine(delayed_pulses());
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
                //call laser aim function if laser attack state is entered
                else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_charge)
                {
                    laser_player_pos = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z);

                    if (transform.right.x >= 0)
                    {
                        
                        laser_manager.aim_laser(new Vector3(transform.position.x + 0.5f, transform.position.y + 1, transform.position.z), 
                           laser_player_pos, 0.2f, 30f);
                    }
                    if (transform.right.x < 0)
                    {
                        laser_manager.aim_laser(new Vector3(transform.position.x - 0.5f, transform.position.y + 1, transform.position.z),
                            laser_player_pos, 0.2f, 30f);
                    }
                }
                //call laser function if laser attack state is entered
                else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_attack)
                {

                    if (transform.right.x >= 0)
                    {
                        laser_manager.fire_laser(new Vector3(transform.position.x + 0.5f, transform.position.y + 1, transform.position.z), true,
                           laser_player_pos, 0.2f, 30f);
                    }
                    if (transform.right.x < 0)
                    {
                        laser_manager.fire_laser(new Vector3(transform.position.x - 0.5f, transform.position.y + 1, transform.position.z), true,
                            laser_player_pos, 0.2f, 30f);
                    }
                }
                //call laser rain aim function of laser rain aim state is entered
                else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_rain_charge)
                {
                    float x = -9f;
                    for (int i = 0; i < 10; i++)
                    {
                        float rand_x = (x - 0.5f) + (Random.value);
                        laser_rain_positions[i] = new Vector3(rand_x, 10, -0.558f);
                        laser_rain_directions[i] = new Vector3(rand_x, -2, -0.558f);
                        x += 2f;

                    }

                    laser_manager.laser_rain_aim(laser_rain_positions, laser_rain_directions);
                }
                //call laser rain attack function if laser rain aim state is entered
                else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_rain_attack)
                {
                    laser_manager.laser_rain_fire(laser_rain_positions, laser_rain_directions);
                }
                //call laser rapid aim function if laser rapid aim state is entered
                else if((state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_rapid_charge))
                {
                    laser_consecutive_fire(true, 0.2f, 10f, 3, 2.25f);
                }

            }

                string desc = Utility_methods.GetDescription<Enemy1_State_Transition_Func.enemy1_state>((Enemy1_State_Transition_Func.enemy1_state)state_controller.curr_state);
                if(desc != ""){
                    enemy_animator.Play(desc);
                }else{
                    enemy_animator.Play("idle");
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

                if (transform.position.x >= 9.5)
                {
                    transform.position = new Vector3(9.5f, transform.position.y, transform.position.z);
                }
                else if (transform.position.x <= -9.5)
                {
                    transform.position = new Vector3(-9.5f, transform.position.y, transform.position.z);
                }
            }
            //enemy is running to the right
            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_right)
            {
                transform.Translate(Time.deltaTime * run_attack_speed, 0, 0);
                if (transform.position.x >= 8)
                {
                    transform.position = new Vector3(8f, transform.position.y, transform.position.z);
                }
            }
            //enemy is running to the left
            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.run_attack_left)
            {
                transform.Translate(Time.deltaTime * run_attack_speed, 0, 0);
                if (transform.position.x <= -8)
                {
                    transform.position = new Vector3(-8f, transform.position.y, transform.position.z);                  
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
                if (transform.position.x >= 8.5f && transform.right.x > 0)
                {
                    transform.position = new Vector3(8.5f, transform.position.y, transform.position.z);
                }
                if (transform.position.x <= -8.5f && transform.right.x < 0)
                {
                    transform.position = new Vector3(-8.5f, transform.position.y, transform.position.z);
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

    
    }
}
