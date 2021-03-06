using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game_Util;

namespace Game_Control
{
    public class Player_controller : MonoBehaviour
    {

        int player_id;

        public float Speed = 5.0f;
        public float JumpHeight = 1.5f;

        private float dodge_horizontal;
        private float dodge_vertical;
        private float dodge_speed = 12.0f;

        private bool parry_stop = false;
        private float parry_stop_initial;

        private float lerpSpeed;

        private bool paused = false;
        private Quaternion rot;

        private CharacterController _character_controller;
        // private Rigidbody rb;
        private Vector3 _velocity;

        public HealthInfo player_health_info;
        public HealthInfo boss_health_info;

        public ComboInfo combo_info;

        public Laser_Manager laser_manager;
        public Pause_Manager pause_manager;
        public Cooldown_Manager cooldown_manager;

        // public Renderer[] AttackVisuals;
        // public Renderer[] PlayerVisuals;


        public Collider[] AttackHitboxes;

        public Renderer PlayerVisuals;
        private Material skin_material;
        private Color skin_natural;



        State_controller state_controller;
        State_controller attack_controller;
        public Vector3 player_state_debug_display;
        public Vector3 attack_state_debug_display;
        // Start is called before the first frame update

        public Animator char_animator;
        void Start()
        {
            _character_controller = GetComponent<CharacterController>();
            _character_controller.minMoveDistance = 0;

            state_controller = new State_controller();
            state_controller.initialize(new Player_State_Transition_Func(_character_controller));

            attack_controller = new State_controller();
            attack_controller.initialize(new Attack_State_Transition_Func());


            rot = Quaternion.Euler(0, 0, 0);

            Material[] materials = PlayerVisuals.materials;

            skin_material = materials[1];

            skin_natural = new Color(1f, 1f, 1f);

            skin_material.SetColor("_Color", skin_natural);

        }
        void OnDrawGizmos()
        {
            if (state_controller != null && attack_controller != null)
            {
                Handles.Label(player_state_debug_display, "movement state: " + (Player_State_Transition_Func.player_state)state_controller.curr_state);
                Handles.Label(attack_state_debug_display, "attack_basic state: " + (Attack_State_Transition_Func.attack_state)attack_controller.curr_state);
            }
        }

        void FixedUpdate()
        {
        }

        // Update is called once per frame
        void Update()
        {
            float horizontal_input = Input.GetAxis("Horizontal");
            float vertical_input = Input.GetAxis("Vertical");
            bool pause_input = Input.GetButtonDown("Pause");

            Player_Input.PlayerInput input = Player_controller_helper.getPlayerInput(ref combo_info);


            pause_game(pause_input);
            if (paused)
            {
                return;
            }

            //if parry_stop is active, check if it should be removed
            if (parry_stop)
            {
                Time.timeScale = 0f;
                if(Time.realtimeSinceStartup - parry_stop_initial > 0.3f)
                {
                    parry_stop = false;
                    Time.timeScale = 1f;
                }
            }

            //reset scene if player dies
            if (player_health_info.is_dead || transform.position.y <= -2)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            reload_scene_if_death();
            
            attack_controller.process_time();
            state_controller.process_time();

            bool state_changed = state_controller.process_state(input);

            process_attack_input(state_changed, input);
            if (attack_controller.curr_state != 0)
            {
                //attacking
                Player_controller_helper.do_attack((Attack_State_Transition_Func.attack_state)attack_controller.curr_state, AttackHitboxes,combo_info,player_health_info,  boss_health_info);
                char_animator.Play(Utility_methods.GetDescription<Attack_State_Transition_Func.attack_state>((Attack_State_Transition_Func.attack_state)attack_controller.curr_state));
            }
            else
            {
                //set animation to whatever else
                char_animator.Play(Utility_methods.GetDescription<Player_State_Transition_Func.player_state>((Player_State_Transition_Func.player_state)state_controller.curr_state));
            }

            apply_movement(horizontal_input, vertical_input);

            update_indicators();

        }


        private void process_attack_input(bool state_changed, Player_Input.PlayerInput input){
            if (input.HasFlag(Player_Input.PlayerInput.Attack))
            {
                if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.attack_basic)
                {
                    attack_controller.process_state(Player_Input.PlayerInput.Attack);
                }
                else if (state_changed)
                {
                    if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.attack_dash)
                    {
                        attack_controller.process_state(Player_Input.PlayerInput.Dash | Player_Input.PlayerInput.Attack);
                    }
                    else if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.attack_special_0)
                    {
                        attack_controller.process_state(Player_Input.PlayerInput.Special_attack_0 | Player_Input.PlayerInput.Attack);
                    }
                }
            }
            else
            {
                attack_controller.process_state(input);
            }

            bool attack_sequence_changed = attack_controller.process_state();

            if (attack_sequence_changed)
            {
                state_controller.state_duration = attack_controller.state_duration;
                if (attack_controller.curr_state != 0 && 
                    attack_controller.curr_state <= (int)Attack_State_Transition_Func.attack_state.attack_basic_4)
                {
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_basic;
                }
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_dash_0)
                {
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_dash;
                }
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_jump_0)
                {
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_jump;
                }

                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_special_0)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    //do attack in correct direction
                    if (transform.right.x >= 0)
                    {
                        laser_manager.fire_laser(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), false, true);
                    }
                    if (transform.right.x < 0)
                    {
                        laser_manager.fire_laser(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), false, false);
                    }
                }
                
            }

        }

        private void reload_scene_if_death(){
            if (player_health_info.is_dead || transform.position.y <= -2)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private void pause_game(bool pause_input){
            if (pause_input && !paused)
            {
                Debug.Log("pause");
                paused = true;
                pause_manager.ShowPause();
                Time.timeScale = 0f;
            }
            else if (pause_input && paused)
            {
                Debug.Log("unpause");
                paused = false;
                pause_manager.RemovePause();
                Time.timeScale = 1f;
            }
        }

        private void apply_movement(float horizontal_input, float vertical_input){
            Vector3 move = new Vector3(0, 0, 0);



            //apply upwards momentum when jump
            if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.jump)
            {
                _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
            }
            else

            //apply dodge movement to the player
            if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.dodge)
            {
                player_health_info.setInvincible(0.2f);
                move += Player_controller_helper.getDodgeVector(horizontal_input, vertical_input) * Time.deltaTime * dodge_speed;

            }
            //otherwise apply regular movement if not in an attack that locks movement
            else if ((attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.not_attacking ||
                    attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_jump_0 ||
                    attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_dash_0) &&
                    (state_controller.curr_state != (int)Player_State_Transition_Func.player_state.parry_active&&
                    state_controller.curr_state != (int)Player_State_Transition_Func.player_state.parry_cooldown))
            {
                //apply horizontal movement
                move += new Vector3(horizontal_input, 0, 0) * Time.deltaTime * Speed;

                // steering the character
                lerpSpeed = 20 * Time.deltaTime;
                if (move.x > 0)
                {
                    rot = Quaternion.Euler(0, 0, 0);
                }

                if (move.x < 0)
                {
                    rot = Quaternion.Euler(0, -180, 0);

                }
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, lerpSpeed);
            }


            apply_gravity();
            //apply all movement
            _character_controller.Move(move + (_velocity * Time.deltaTime));


        }

        private void apply_gravity(){
                        //always apply gravity
            if (state_controller.curr_state != (int)Player_State_Transition_Func.player_state.dodge)
            {
                _velocity.y += Physics.gravity.y * Time.deltaTime * 2;
            }

            //stop downward movement when character lands
            if (_character_controller.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -0.001f;
            }


        }

        private void update_indicators()
        {
            if (cooldown_manager.dodge_ready && state_controller.cooldown_timers[0].Value > 0)
            {
                cooldown_manager.dodge_ready = false;
            }
            else if (!cooldown_manager.dodge_ready && state_controller.cooldown_timers[0].Value <= 0)
            {
                cooldown_manager.dodge_ready = true;
            }

            if (cooldown_manager.laser_ready && (state_controller.cooldown_timers[1].Value > 0 || !combo_info.canFireLaser()))
            {
                cooldown_manager.laser_ready = false;
            }
            else if (!cooldown_manager.dodge_ready && state_controller.cooldown_timers[1].Value <= 0 && combo_info.canFireLaser())
            {
                cooldown_manager.laser_ready = true;
            }
        }
    }
}
