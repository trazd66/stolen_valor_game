using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game_Control{

    public class Enemy1_controller : MonoBehaviour
    {
        State_controller state_controller;

        float run_attack_speed = 5f;
        float front_attack_speed = 1.5f;
        float laser_attack_speed = 8f;

        public GameObject[] AttackObjects;


        public Collider[] AttackHitboxes;


        // Start is called before the first frame update
        void Start()
        {
            state_controller = new State_controller();
            state_controller.initialize(new Enemy1_State_Transition_Func());

        }

        // Update is called once per frame
        void Update()
        {
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
                //activate front attack hitboxes if front attack state has been entered
                else if(state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.front_attack)
                {
                    AttackObjects[0].SetActive(true);
                }
                //remove front attack hitboxes whehn attack ends, and reset them to initial position
                else if(state_controller.prev_states[state_controller.prev_states.Count - 1] == (int)Enemy1_State_Transition_Func.enemy1_state.front_attack)
                {
                    AttackObjects[0].transform.localPosition = new Vector3(1.5f, 0.9f, 0f);
                    AttackObjects[0].SetActive(false);
                }
                //activate laser attack hitboxes if front attack state has been entered
                else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_attack)
                {
                    AttackObjects[1].SetActive(true);
                }
                //remove laser attack hitboxes whehn attack ends, and reset them to initial position
                else if (state_controller.prev_states[state_controller.prev_states.Count - 1] == (int)Enemy1_State_Transition_Func.enemy1_state.laser_attack)
                {
                    AttackObjects[1].transform.localPosition = new Vector3(0.7f, 0.75f, 0f);
                    AttackObjects[1].SetActive(false);
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
                AttackObjects[0].transform.Translate(0, -Time.deltaTime * front_attack_speed, 0);
                //TODO collision checking and damage calculation
            }

            else if (state_controller.curr_state == (int)Enemy1_State_Transition_Func.enemy1_state.laser_attack)
            {
                AttackObjects[1].transform.Translate(Time.deltaTime * laser_attack_speed, 0, 0);
            }
        }
    }
}
