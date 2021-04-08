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

        public bool enable_control;

        //true to enable attacking
        public bool enable_attacks;

        //true to enable parry
        public bool enable_parry;

        //true to enable dodge
        public bool enable_dodge;

        //true to enable laser; different from the laser unlock toggle, this one is needed to prevent the code that checks for the laser indicator from crashing. 
        //setting it to true does not nessecarily unlock laser, that toggle is in comboinfo
        public bool enable_laser;
        private float Speed = 6.0f;
        private float JumpHeight = 2.5f;

        private float dodge_horizontal;
        private float dodge_vertical;
        private float dodge_speed = 15.0f;

        private float dodge_invuln_timer = 0f;

        private float parry_active_duration = 0.2f;
        private float parry_bonus_duration = 3f;

        private bool parry_stop = false;
        private float parry_stop_initial;

        private float lerpSpeed;

        private Quaternion rot;

        private CharacterController _character_controller;
        // private Rigidbody rb;
        private Vector3 _velocity;

        private bool pause_visual = true;
        private bool control_visual = false;

        private float knockback_timer = 0f;
        private float knockback_speed = 0f;
        private bool knockback_direction_right = false;

        public HealthInfo player_health_info;
        public HealthInfo boss_health_info;

        public ComboInfo combo_info;
        public GameObject cannon;
        public Laser_Manager laser_manager;
        public Pause_Manager pause_manager;
        public Cooldown_Manager cooldown_manager;

        public GameObject game_over;
        public GameObject win_screen;

        private bool win = false;

        public Collider[] AttackHitboxes;

        public Renderer PlayerVisuals;
        private Material skin_material;
        private Color skin_natural;
        private Color parry_active_colour = new Color(0f, 1f, 1f);
        private Color parry_cooldown_colour = new Color(0f, 0f, 0f);
        private Color parry_success_colour = new Color(0f, 1f, 0f);
        private Color invincible_colour = new Color(1f, 1f, 1f, 0.0f);
        private Color hurt_colour = new Color(1f, 0f, 0f, 0.5f);

        private Vector3 move = new Vector3(0, 0, 0);

        State_controller state_controller;
        State_controller attack_controller;

        public Animator char_animator;

        private Attack_State_Transition_Func attack_transitions;

        public Attack_State_Transition_Func.attack_state curr_atk_state
        {
            get
            {
                return (Attack_State_Transition_Func.attack_state)attack_controller.curr_state;
            }
        }
        public bool player_alive
        {
            get
            {
                return player_health_info.curr_health > 0;
            }
        }

        private bool initial_boost = false;
        private bool delayed_boost = false;
        //return true if the direction to apply knockback in is right, false otherwise
        public bool get_knockback_direction_right()
        {
            return transform.right.x < 0;
        }

        //apply knockback, the intensity of which is based off the damage done
        public void apply_knockback(bool direction_right, int damage)
        {
            knockback_direction_right = direction_right;
            knockback_speed = 5f + (float)(damage) / 5f;
            knockback_timer = 0.25f;
        }

        public Player_State_Transition_Func.player_state get_curr_state
        {
            get
            {
                return (Player_State_Transition_Func.player_state)state_controller.curr_state;
            }
        }

        public void set_win()
        {
            win = true;
        }


        void Awake()
        {
            _character_controller = GetComponent<CharacterController>();
            _character_controller.minMoveDistance = 0;

            state_controller = new State_controller();
            state_controller.initialize(new Player_State_Transition_Func(_character_controller, player_health_info));

            attack_controller = new State_controller();
            attack_transitions = new Attack_State_Transition_Func();
            attack_controller.initialize(attack_transitions);


            rot = Quaternion.Euler(0, 0, 0);

            Material[] materials = PlayerVisuals.materials;

            skin_material = materials[1];

            skin_natural = skin_material.color;

            AudioManager.instance.SetLoop("char_hoveridle", true);
            AudioManager.instance.Play("char_hoveridle");

        }


        public IEnumerator Flash(Color c)

        {
            skin_material.color = c;
            yield return new WaitForSeconds(0.05f);
            skin_material.color = skin_natural;
            StopCoroutine("Flash");
        }


        void FixedUpdate()
        {
        }

        public void improve_attack_interval(){
                attack_transitions.improve_attack_interval(2f);
                StartCoroutine(slow_attack_interval());
        }
        public IEnumerator slow_attack_interval()

        {
            yield return new WaitForSeconds(2f);
            attack_transitions.improve_attack_interval(0.5f);
            StopCoroutine(slow_attack_interval());
        }
        // Update is called once per frame
        void Update()
        {

            if (!enable_control)
            {
                return;
            }

            float horizontal_input = Input.GetAxis("Horizontal");
            float vertical_input = Input.GetAxis("Vertical");
            bool pause_input = Input.GetButtonDown("Pause");

            reload_scene_if_death();

            if (win)
            {
                win_screen.SetActive(true);
                AudioManager.instance.Stop("char_hoveridle");
                if (Input.GetButtonDown("Jump"))
                {
                    Time.timeScale = 1f;
                    Game_Manager.instance.setState(5);
                }

            }

            Player_Input.PlayerInput input = Player_controller_helper.getPlayerInput(ref combo_info, enable_attacks, enable_parry, enable_dodge, enable_laser);

            if (pause_manager.GetLaserPaused())
            {
                if (Input.GetButtonDown("Jump"))
                {
                    pause_manager.UnpauseLaser();
                    pause_manager.RemoveLaserTutorial();
                    pause_manager.PauseCombo();
                    pause_manager.ShowComboTutorial();
                }
            }

            else if (pause_manager.GetComboPaused())
            {
                if (Input.GetButtonDown("Jump"))
                {
                    pause_manager.UnpauseCombo();
                    pause_manager.RemoveComboTutorial();
                    Time.timeScale = 1f;
                }

                if (Input.GetButtonDown("Attack"))
                {
                    pause_manager.UnpauseCombo();
                    pause_manager.RemoveComboTutorial();
                    pause_manager.PauseLaser();
                    pause_manager.ShowLaserTutorial();
                }
            }

            pause_game(pause_input);

            if (pause_manager.GetPaused())
            {
                if (Input.GetButtonDown("Jump") && pause_visual)
                {
                    pause_manager.RemovePause();
                    pause_visual = false;
                }
                else if (Input.GetButtonDown("Jump") && !pause_visual)
                {
                    pause_manager.ShowPause();
                    pause_visual = true;
                }
                else if(Input.GetButtonDown("Attack") && pause_visual && !control_visual)
                {
                    pause_manager.RemovePause();
                    pause_manager.ShowControls();
                    pause_visual = false;
                    control_visual = true;
                }
                else if (Input.GetButtonDown("Attack") && control_visual && !pause_visual)
                {
                    pause_manager.RemoveControls();
                    pause_manager.ShowPause();
                    pause_visual = true;
                    control_visual = false;
                }
                else if (Input.GetButtonDown("Dodge") && pause_visual)
                {
                    Time.timeScale = 1f;
                    AudioManager.instance.Stop("char_hoveridle");
                    Game_Manager.instance.setState(5);
                }
                return;
            }

            //if parry_stop is active, check if it should be removed
            if (parry_stop)
            {
                Time.timeScale = 0f;
                if (Time.realtimeSinceStartup - parry_stop_initial > 0.3f)
                {
                    parry_stop = false;
                    Time.timeScale = 1f;
                }
            }

            attack_controller.process_time();
            state_controller.process_time();

            bool state_changed = state_controller.process_state(input);

            process_attack_input(state_changed, input);
            if (attack_controller.curr_state != 0)
            {
                //attacking
                char_animator.Play(Utility_methods.GetDescription<Attack_State_Transition_Func.attack_state>((Attack_State_Transition_Func.attack_state)attack_controller.curr_state));
            }
            else
            {
                string desc = Utility_methods.GetDescription<Player_State_Transition_Func.player_state>((Player_State_Transition_Func.player_state)state_controller.curr_state);
                if (desc != "")
                {
                    char_animator.Play(desc);
                }
                //set animation to whatever else
            }

            process_parry(state_changed);

            setColour();

            apply_movement(horizontal_input, vertical_input);

            update_indicators();

            if (dodge_invuln_timer > 0)
            {
                dodge_invuln_timer -= Time.deltaTime;
            }

            if (knockback_timer > 0)
            {
                knockback_timer -= Time.deltaTime;
            }

        }

        public void enable_gameplay()
        {
            enable_control = true;
            enable_attacks = true;
            enable_parry = true;
            enable_dodge = true;
            enable_laser = true;
        }

        private void process_parry(bool state_changed)
        {
            if (state_changed && state_controller.curr_state == (int)Player_State_Transition_Func.player_state.parry_active)
            {
                player_health_info.setParryReady(parry_active_duration);
                player_health_info.setInvincible(parry_active_duration);
            }
            if (player_health_info.getParrySuccess())
            {
                AudioManager.instance.Play("parry_success");
                player_health_info.setParryReady(0f);
                player_health_info.setInvincible(0.5f);
                player_health_info.setParryBonus(parry_bonus_duration);
                player_health_info.setParrySuccess(false);
            }
        }

        private void process_attack_input(bool state_changed, Player_Input.PlayerInput input)
        {
            if (input.HasFlag(Player_Input.PlayerInput.Attack))
            {
                if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.attack_basic)
                {
                    attack_controller.process_state(Player_Input.PlayerInput.Attack);
                }
                else if (state_changed)
                {
                    if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.attack_jump)
                    {
                        attack_controller.process_state(Player_Input.PlayerInput.Jump | Player_Input.PlayerInput.Attack);
                    }
                    else if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.attack_dash)
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
                    attack_controller.curr_state <= (int)Attack_State_Transition_Func.attack_state.attack_basic_3)
                {
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_basic;
                }else 
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_dash_0)
                {
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_dash;
                }else
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_jump_0)
                {
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_jump;
                }
                
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_basic_3){
                    initial_boost = true;
                    delayed_boost = false;
                }
                
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_special_0)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    //do attack in correct direction
                    if (transform.right.x >= 0)
                    {
                        laser_manager.fire_laser(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), false,
                            new Vector3(transform.position.x + 10, transform.position.y + 1, transform.position.z), 0.2f, 10f);
                    }
                    if (transform.right.x < 0)
                    {
                        laser_manager.fire_laser(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), false,
                            new Vector3(transform.position.x - 10, transform.position.y + 1, transform.position.z), 0.2f, 10f);
                    }
                }

            }

        }

        private void setColour()
        {
            if (player_health_info.parry_ready)
            {
                skin_material.SetColor("_Color", parry_active_colour);
            }
            else if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.parry_cooldown)
            {
                skin_material.SetColor("_Color", parry_cooldown_colour);
            }
            else if (player_health_info.parry_bonus)
            {
                skin_material.SetColor("_Color", parry_success_colour);
            }
            else if (dodge_invuln_timer > 0)
            {
                StartCoroutine("Flash", invincible_colour);
            }
            else if (player_health_info.is_invincible)
            {
                StartCoroutine("Flash", hurt_colour);
            }
            else
            {
                skin_material.SetColor("_Color", skin_natural);
            }
        }

        private void reload_scene_if_death()
        {
            //reset scene if player dies
            if (player_health_info.is_dead || transform.position.y <= -2)
            {
                game_over.SetActive(true);
                if (Input.GetButtonDown("Jump"))
                {
                    Time.timeScale = 1f;
                    AudioManager.instance.Stop("char_hoveridle");
                    Game_Manager.instance.setState(5);
                }

            }
        }

        
        private void pause_game(bool pause_input)
        {
            if (pause_input && !pause_manager.GetPaused())
            {
                Debug.Log("pause");
                pause_manager.PauseGame();
                pause_manager.ShowPause();
                Time.timeScale = 0f;
            }
            else if (pause_input && pause_manager.GetPaused())
            {
                Debug.Log("unpause");
                pause_manager.UnpauseGame();
                pause_manager.RemovePause();
                pause_manager.RemoveControls();
                pause_visual = true;
                control_visual = false;
                Time.timeScale = 1f;
            }
        }

        public IEnumerator wait_for_boost()

        {
            initial_boost = false;
            yield return new WaitForSeconds(0.2f);
            delayed_boost = true;
        }

        private void apply_movement(float horizontal_input, float vertical_input)
        {
            move = new Vector3(0, 0, 0);



            //apply upwards momentum when jump
            if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.jump)
            {
                _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
                AudioManager.instance.Play("JUMP");
            }
            else

            //apply backwards momentum when knocked back
            if (knockback_timer > 0)
            {
                if (knockback_direction_right)
                {
                    move += new Vector3(1, 0, 0) * Time.deltaTime * knockback_speed;
                }
                else
                {
                    move += new Vector3(-1, 0, 0) * Time.deltaTime * knockback_speed;
                }

            }
             if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_basic_3){
                if(initial_boost == true && delayed_boost == false){                
                    StartCoroutine(wait_for_boost());
                }else
                if(initial_boost == false && delayed_boost == true){
                    move += new Vector3(0, 1, 0) * 0.5f * Time.deltaTime * dodge_speed;
                }else{
                    delayed_boost = false;
                }

             }
  

            //apply dodge movement to the player
            if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.dodge)
            {
                player_health_info.setInvincible(0.2f);
                dodge_invuln_timer = 0.2f;
                move += Player_controller_helper.getDodgeVector(horizontal_input, vertical_input) * Time.deltaTime * dodge_speed;
                AudioManager.instance.Play("DODGE");

            }
            //otherwise apply regular movement if not in an attack that locks movement
            else if ((attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.not_attacking ||
                    attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_jump_0 ||
                    attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_dash_0) &&
                    (state_controller.curr_state != (int)Player_State_Transition_Func.player_state.parry_active &&
                    state_controller.curr_state != (int)Player_State_Transition_Func.player_state.parry_cooldown) &&
                    knockback_timer <= 0)
            {
                //apply horizontal movement
                move += new Vector3(horizontal_input, 0, 0) * Time.deltaTime * Speed;

                // steering the character
                if (attack_controller.curr_state != (int)Attack_State_Transition_Func.attack_state.attack_jump_0)
                {
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

            }


            apply_gravity();
            //apply all movement
            _character_controller.Move(move + (_velocity * Time.deltaTime));


        }

        private void apply_gravity()
        {
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

            if (enable_laser)
            {
                if (cooldown_manager.laser_ready && !combo_info.canFireLaser())
                {
                    cooldown_manager.laser_ready = false;
                }
                else if (!cooldown_manager.laser_ready && combo_info.canFireLaser())
                {
                    cooldown_manager.laser_ready = true;
                }
            }
        }
    }
}