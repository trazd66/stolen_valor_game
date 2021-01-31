using System.Collections;
using System.Collections.Generic;

namespace Game_Control{



/*
The player and the enemy are always in a state (pretty similar to animation states, but it contains other informaiton as well)
*/

public enum player_state{
    idle,
    walking,
    walking_backward,
    turning,
    running,
    jumping,
    attacking,
    dodging,
    attacked,
    parrying,
    parry_success,
    parry_fail
}

public enum player_attack_state{
    basic,
    special_0,
    special_1,
    special_2
}



/// <summary>
///     State transition function for the player
/// </summary>
public class Player_State_Transition_Func : IState_Transition_Func{
    

    /// <summary>
    /// TODO: the state transition function should be based on input
    /// e.g. transition from attacking to parrying
    /// however, there should also be limitations on transitions, 
    /// e.g. we should wait for the current animation to finish before allowing a transition
    /// </summary>
    /// <param name="curr_state"></param>
    /// <param name="prev_states"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public int process_state_with_player_input(int curr_state,List<int> prev_states, Player_Input input){
        return curr_state;
    }


    /// <summary>
    /// probably don't need this for the player
    /// </summary>
    /// <param name="curr_state"></param>
    /// <param name="prev_states"></param>
    /// <returns></returns>
    public int process_state(int curr_state,List<int> prev_states){
        return curr_state;
    }


}

}