using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game_Control{

    public class Enemy1_controller : MonoBehaviour
    {
        State_controller state_controller;

        // Start is called before the first frame update
        void Start()
        {
            state_controller = new State_controller();
            state_controller.initialize(new Enemy1_State_Transition_Func());
        }

        // Update is called once per frame
        void Update()
        {
            state_controller.process_state();
        }
    }
}
