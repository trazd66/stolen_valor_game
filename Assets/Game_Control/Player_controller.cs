using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Game_Control{
    public class Player_controller : MonoBehaviour
    {

        int player_id;

        public float Speed = 10.0f;
        public float JumpHeight = 2f;


        private CharacterController _character_controller;
        private Vector3 _velocity;


        State_controller state_controller;

        // Start is called before the first frame update
        void Start()
        {
            state_controller = new State_controller();
            state_controller.initialize(new Player_State_Transition_Func());

            _character_controller = GetComponent<CharacterController>();


            }

        void FixedUpdate(){
        }

        // Update is called once per frame
        void Update()
        {

            // state_controller.process_state()
            // actually move the player 
            // every frame, we need to read plyaer input
            // and based on the player input, we transition states
            // and also move the player

            List<Player_Input.PlayerInput> inputs = new List<Player_Input.PlayerInput>();

            //checked if left or right are inputed
            if (Input.GetAxis("Horizontal") != 0)
            {
                inputs.Add(Player_Input.PlayerInput.Walk);
            }

            //check if jump is inputed
            if (Input.GetButtonDown("Jump"))
            {
                inputs.Add((Player_Input.PlayerInput.Jump));
            }

            //get a single PlayerInput to represent the totality of inputs
            Player_Input.PlayerInput total_input = Player_Input.get_input(inputs);

            //call the appropriate process_state
            if (total_input == Player_Input.PlayerInput.None)
            {
                state_controller.process_state();
            }
            else
            {
                state_controller.process_state(total_input);
            }

            //apply movement to the player

            //stop downward movement when character lands
            if (_character_controller.isGrounded && _velocity.y < 0)
                _velocity.y = 0f;

            //apply upwards momentum when jumping
            if (Input.GetButtonDown("Jump"))
            {
                _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
            }

            //apply horizontal movement
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            _character_controller.Move(move * Time.deltaTime * Speed);

            // steering the character
            if (move != Vector3.zero)
                transform.right = move;

            //apply gravity
            _velocity.y += Physics.gravity.y * Time.deltaTime * 2;
            _character_controller.Move(_velocity * Time.deltaTime);

            //for debugging
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log(state_controller.curr_state);
            }

        }
    }


}
