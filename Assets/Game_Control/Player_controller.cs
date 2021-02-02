using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Game_Control{
    public class Player_controller : MonoBehaviour
    {

        int player_id;

        private CharacterController _character_controller;


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





        }

    }


}
