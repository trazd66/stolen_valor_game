using System.Collections;
using System.Collections.Generic;


namespace Game_Control{
/*
    Interface that serves as a template on the transition functions
    implement this interface for the boss AI and player state transitions
*/
public interface IState_Transition_Func{
    //Given the current state and the previous state and what the player input is, determine what the next state is
    int process_state_with_player_input(int curr_state,List<int> prev_states, Player_Input input);
    int process_state(int curr_state,List<int> prev_states);
}


}