using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


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

        public Renderer[] AttackVisuals;
        public Collider[] AttackHitboxes;
        public Renderer[] PlayerVisuals;

        State_controller state_controller;
        State_controller attack_controller;

        public GameObject additional_part_0;
        public Vector3 player_state_debug_display;
        public Vector3 attack_state_debug_display;
        // Start is called before the first frame update
        void Start()
        {
            _character_controller = GetComponent<CharacterController>();
            _character_controller.minMoveDistance = 0;

            state_controller = new State_controller();
            state_controller.initialize(new Player_State_Transition_Func(_character_controller));

            attack_controller = new State_controller();
            attack_controller.initialize(new Attack_State_Transition_Func());

            //remove this when someone figures out how to change the default colour in unity lol
            for (int i = 0; i < PlayerVisuals.Length; i++)
            {
                PlayerVisuals[i].material.SetColor("_Color", Color.green);
            }
            rot = Quaternion.Euler(0, 0, 0);

        }
        void OnDrawGizmos()
        {
            if (state_controller != null && attack_controller != null)
            {
                //Handles.Label(player_state_debug_display, "movement state: " + (Player_State_Transition_Func.player_state)state_controller.curr_state);
                //Handles.Label(attack_state_debug_display, "attack_basic state: " + (Attack_State_Transition_Func.attack_state)attack_controller.curr_state);
            }
        }

        void FixedUpdate()
        {
        }

        // Update is called once per frame
        void Update()
        {

            Player_Input.PlayerInput input = Player_controller_helper.getPlayerInput(ref combo_info);
            if (Input.GetKeyDown("f"))
            {
                Debug.Log(state_controller.curr_state);
                Debug.Log(attack_controller.curr_state);

            }

            if (Input.GetButtonDown("Pause") && !paused)
            {
                Debug.Log("pause");
                paused = true;
                pause_manager.ShowPause();
                Time.timeScale = 0f;
            }
            else if (Input.GetButtonDown("Pause") && paused)
            {
                Debug.Log("unpause");
                paused = false;
                pause_manager.RemovePause();
                Time.timeScale = 1f;
            }

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

            attack_controller.process_time();
            state_controller.process_time();

            bool state_changed = state_controller.process_state(input);

            //call attack state transition function if attack is initiated
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
            }else
            {
                attack_controller.process_state(input);
            }

            bool attack_sequence_changed = attack_controller.process_state();
            if (attack_sequence_changed)
            {
                //perhaps this should be a helper function in the future

                //if the previous attack state is from basic attack, then remove whatever visuals are left over from the previous attack
                if (attack_controller.prev_states.Count > 0 && attack_controller.prev_states[attack_controller.prev_states.Count - 1] == (int)Attack_State_Transition_Func.attack_state.attack_basic_0)
                {
                    AttackVisuals[0].enabled = false;
                }
                else
                if (attack_controller.prev_states.Count > 0 && attack_controller.prev_states[attack_controller.prev_states.Count - 1] == (int)Attack_State_Transition_Func.attack_state.attack_basic_1)
                {
                    AttackVisuals[0].enabled = false;
                }
                else
                if (attack_controller.prev_states.Count > 0 && attack_controller.prev_states[attack_controller.prev_states.Count - 1] == (int)Attack_State_Transition_Func.attack_state.attack_basic_2)
                {
                    AttackVisuals[0].enabled = false;
                }
                else
                if (attack_controller.prev_states.Count > 0 && attack_controller.prev_states[attack_controller.prev_states.Count - 1] == (int)Attack_State_Transition_Func.attack_state.attack_basic_3)
                {
                    AttackVisuals[0].enabled = false;
                }
                else
                if (attack_controller.prev_states.Count > 0 && attack_controller.prev_states[attack_controller.prev_states.Count - 1] == (int)Attack_State_Transition_Func.attack_state.attack_basic_4)
                {
                    AttackVisuals[0].enabled = false;
                }
                else
                if (attack_controller.prev_states.Count > 0 && attack_controller.prev_states[attack_controller.prev_states.Count - 1] == (int)Attack_State_Transition_Func.attack_state.attack_dash_0)
                {
                    AttackVisuals[5].enabled = false;
                }
                else
                if (attack_controller.prev_states.Count > 0 && attack_controller.prev_states[attack_controller.prev_states.Count - 1] == (int)Attack_State_Transition_Func.attack_state.attack_dash_1)
                {
                    AttackVisuals[6].enabled = false;
                }
                else
                if (attack_controller.prev_states.Count > 0 && attack_controller.prev_states[attack_controller.prev_states.Count - 1] == (int)Attack_State_Transition_Func.attack_state.attack_dash_2)
                {
                    AttackVisuals[7].enabled = false;
                }


                //launch attack if new state is basic attack
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_basic_0)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_basic;
                    //do attack
                    Player_controller_helper.do_attack((Attack_State_Transition_Func.attack_state)attack_controller.curr_state, AttackVisuals, AttackHitboxes, combo_info, player_health_info, boss_health_info);
                }
                else
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_basic_1)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_basic;
                    //do attack
                    Player_controller_helper.do_attack((Attack_State_Transition_Func.attack_state)attack_controller.curr_state, AttackVisuals, AttackHitboxes, combo_info, player_health_info, boss_health_info);
                }
                else
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_basic_2)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_basic;
                    //do attack
                    Player_controller_helper.do_attack((Attack_State_Transition_Func.attack_state)attack_controller.curr_state, AttackVisuals, AttackHitboxes, combo_info, player_health_info, boss_health_info);
                }
                else
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_basic_3)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_basic;
                    //do attack
                    Player_controller_helper.do_attack((Attack_State_Transition_Func.attack_state)attack_controller.curr_state, AttackVisuals, AttackHitboxes, combo_info, player_health_info, boss_health_info);
                }
                else
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_basic_4)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_basic;
                    //do attack
                    Player_controller_helper.do_attack((Attack_State_Transition_Func.attack_state)attack_controller.curr_state, AttackVisuals, AttackHitboxes, combo_info, player_health_info, boss_health_info);
                }
                else
                //launch dash attack
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_dash_0)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_dash;
                    //do attack
                    Player_controller_helper.do_attack((Attack_State_Transition_Func.attack_state)attack_controller.curr_state, AttackVisuals, AttackHitboxes, combo_info, player_health_info, boss_health_info);
                }
                else
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_dash_1)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_dash;
                    //do attack
                    Player_controller_helper.do_attack((Attack_State_Transition_Func.attack_state)attack_controller.curr_state, AttackVisuals, AttackHitboxes, combo_info, player_health_info, boss_health_info);
                }
                else
                if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_dash_2)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    state_controller.curr_state = (int)Player_State_Transition_Func.player_state.attack_dash;
                    //do attack
                    Player_controller_helper.do_attack((Attack_State_Transition_Func.attack_state)attack_controller.curr_state, AttackVisuals, AttackHitboxes, combo_info, player_health_info, boss_health_info);
                }
                //launch laser attack
                else if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_special_0)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    //do attack in correct direction
                    if (transform.right.x >= 0)
                    {
                        laser_manager.fire_laser(transform.position, false, true);
                    }
                    if (transform.right.x < 0)
                    {
                        laser_manager.fire_laser(transform.position, false, false);
                    }
                }
            }


            // if(attack_controller.curr_state != (int) Attack_State_Transition_Func.attack_state.not_attacking && !(ui.get_enemy_invincible())){
            //     Collider col  = AttackHitboxes[0];
            //      //check what Colliders on the PlayerHitbox layer overlap col
            //     Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("EnemyHitbox"));
            //     if(cols.Length > 0){
            //         Debug.Log("hit");
            //         //cols[0].gameObject.GetComponentInParent<HealthInfo>().curr_health -= 10;
            //         ui.BossDamage(10f);
            //     }
            // }

            //activate parry state
            if (state_changed && state_controller.curr_state == (int)Player_State_Transition_Func.player_state.parry_active)
            {
                Debug.Log("parry activated");
                player_health_info.setInvincible(0.1f);
                player_health_info.setParryReady(0.1f);
            }

            //process successful parry
            if (player_health_info.getParrySuccess())
            {
                Debug.Log("parry_success");
                player_health_info.setParrySuccess(false);
                player_health_info.setParryReady(0.0f);
                player_health_info.setInvincible(0.5f);
                player_health_info.setParryBonus(2.0f);
                state_controller.curr_state = (int)Player_State_Transition_Func.player_state.idle;
                state_controller.state_duration = 0f;
                parry_stop = true;
                parry_stop_initial = Time.realtimeSinceStartup;
            }

            //apply colour
            if (player_health_info.parry_bonus)
            {
                for (int i = 0; i < PlayerVisuals.Length; i++)
                {
                    PlayerVisuals[i].material.SetColor("_Color", Color.green);
                }
            }
            else
            if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.parry_active)
            {
                for (int i = 0; i < PlayerVisuals.Length; i++)
                {
                    PlayerVisuals[i].material.SetColor("_Color", Color.cyan);
                }
            }
            else
            if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.parry_cooldown)
            {
                for (int i = 0; i < PlayerVisuals.Length; i++)
                {
                    PlayerVisuals[i].material.SetColor("_Color", Color.black);
                }
            }
            else
            if (player_health_info.is_invincible)
            {
                for (int i = 0; i < PlayerVisuals.Length; i++)
                {
                    PlayerVisuals[i].material.SetColor("_Color", Color.white);
                }
            } 
            else
            {
                for (int i = 0; i < PlayerVisuals.Length; i++)
                {
                    PlayerVisuals[i].material.SetColor("_Color", Color.blue);
                }
            }

            Vector3 move = new Vector3(0, 0, 0);



            //apply upwards momentum when jump
            if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.jump && state_changed)
            {
                _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
            }else

            //apply dodge movement and colourto the player
            if (state_controller.curr_state == (int)Player_State_Transition_Func.player_state.dodge)
            {
                player_health_info.setInvincible(0.15f);
                move += Player_controller_helper.getDodgeVector(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * dodge_speed;

            }
            //otherwise apply regular movement
            else if ((attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.not_attacking || 
                attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.attack_jump_0) &&
                state_controller.curr_state != (int)Player_State_Transition_Func.player_state.parry_active &&
                state_controller.curr_state != (int)Player_State_Transition_Func.player_state.parry_cooldown)
            {
                //apply horizontal movement
                move += new Vector3(Input.GetAxis("Horizontal"), 0, 0) * Time.deltaTime * Speed;

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

            //lerprot = move.x;
            //Quaternion right_rot = Quaternion.Euler(0, 0, 0);
            //Quaternion left_rot = Quaternion.Euler(0, -180, 0);
            //if (move.x > 0)
            //{
            //transform.rotation = Quaternion.Slerp(transform.rotation, right_rot, lerpSpeed);
            //}

            //if (move.x < 0)
            //{
            //transform.rotation = Quaternion.Slerp(transform.rotation, left_rot, lerpSpeed);
            //}
            //Debug.Log(transform.right.x);


            //apply gravity
            if (state_controller.curr_state != (int)Player_State_Transition_Func.player_state.dodge)
            {
                _velocity.y += Physics.gravity.y * Time.deltaTime * 2;
            }

            //stop downward movement when character lands
            if (_character_controller.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -0.001f;
            }

            //apply all movement
            _character_controller.Move(move + (_velocity * Time.deltaTime));



        }

    }


}
