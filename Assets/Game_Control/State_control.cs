using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game_Control{

public class State_controller{

    public int curr_state;

    /// <summary>
    /// the state timer determines how long the current state lasts
    /// </summary>
    public float state_duratin;

    public List<int> prev_states;

    public State_controller(){
        prev_states = new List<int>(200); //TODO: make a new class to use here instead of List, that automatically removes old states from the bottom of the stack so that we don't waste
                                          //memory holding thousands of previous states
    }

    public void initialize(IState_Transition_Func transition_function){
        this.transition_function = transition_function;
        transition_function.initialize(ref curr_state,ref prev_states,ref state_duratin);
    }


    private IState_Transition_Func transition_function;


    /*
        Process the state controller using the transition function
     */
    public bool process_state(){
        return transition_function.process_state(ref curr_state,ref prev_states,ref state_duratin);
    }

    /*
        Process the state controller using the transition function with the player input
     */
    public bool process_state(Player_Input.PlayerInput input){
        return transition_function.process_state_with_player_input(ref curr_state,ref prev_states,ref state_duratin,input);

    }
    
}


}
