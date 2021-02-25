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


        private CharacterController _character_controller;
        // private Rigidbody rb;
        private Vector3 _velocity;

        public HealthInfo player_health_info;
        public HealthInfo boss_health_info;

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

            Player_Input.PlayerInput input = Player_controller_helper.getPlayerInput();
            if (Input.GetKeyDown("f"))
            {
                Debug.Log("debug key put info here");
            }

            if (player_health_info.is_dead)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            attack_controller.process_time();
            state_controller.process_time();
            //call process_state
            // state_controller.process_state();
            bool state_changed = state_controller.process_state(input);


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
                }
            }else
            {
                attack_controller.process_state(input);
            }

            bool attack_sequence_changed = attack_controller.process_state();
            if (attack_sequence_changed)
            {
                //if the previous attack state is NOT not_attacking, then remove whatever visuals are left over from the previous attack
                if (attack_controller.prev_states.Count > 0 && attack_controller.prev_states[attack_controller.prev_states.Count - 1] != (int)Attack_State_Transition_Func.attack_state.not_attacking)
                {
                    AttackVisuals[0].enabled = false;
                }
                //launch attack if new state is NOT not_attacking
                if (attack_controller.curr_state != (int)Attack_State_Transition_Func.attack_state.not_attacking)
                {
                    state_controller.state_duration = attack_controller.state_duration;
                    //do attack
                    Player_controller_helper.do_attack((Attack_State_Transition_Func.attack_state)attack_controller.curr_state, AttackVisuals, AttackHitboxes, boss_health_info);
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


            //apply colour
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
                    PlayerVisuals[i].material.SetColor("_Color", Color.green);
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
                player_health_info.setInvincible(0.2f);
                move += Player_controller_helper.getDodgeVector(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * dodge_speed;

            }
            //otherwise apply regular movement
            else if (attack_controller.curr_state == (int)Attack_State_Transition_Func.attack_state.not_attacking)
            {
                //apply horizontal movement
                move += new Vector3(Input.GetAxis("Horizontal"), 0, 0) * Time.deltaTime * Speed;

                // steering the character
                if (move != Vector3.zero)
                    transform.right = move;
            }



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
