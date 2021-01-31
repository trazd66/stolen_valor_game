using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game_Control{

public class State_controller{

    public int curr_state{get; private set;}

    public List<int> prev_states{get; private set;}



    private IState_Transition_Func transition_function{get; set;}


    /*
        Process the state controller using the transition function
     */
    public int process_state(){
        int new_state = transition_function.process_state(curr_state,prev_states);
        prev_states.Add(curr_state);
        curr_state = new_state;        
        return curr_state;
    }

    /*
        Process the state controller using the transition function with the player input
     */
    public int process_state(Player_Input input){
        int new_state = transition_function.process_state_with_player_input(curr_state,prev_states,input);
        prev_states.Add(curr_state);
        curr_state = new_state;        
        return curr_state;
    }
    
}


}
