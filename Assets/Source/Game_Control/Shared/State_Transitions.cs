using System.Collections;
using System.Collections.Generic;
using Game_Util;

namespace Game_Control{
/*
    Interface that serves as a template on the transition functions
    implement this interface for the boss AI and player state transitions
*/
public interface IState_Transition_Func{


    /// <summary>
    ///  Takes in a reference of the current state and the previous_states and the duration variable and the player input
    ///  changes the states based on the input
    ///  will modify the curr_state,prev_states and duration variable if state is changed
    /// </summary>
    /// <param name="curr_state">the current state </param>
    /// <param name="prev_states"> list of previous states </param>
    /// <param name="duration"> duration of the state </param>
    /// <param name="input"> player input</param>
    /// <returns>true if the state has been changed </returns>
    bool process_state_with_player_input(ref int curr_state, ref List<int> prev_states, ref float duration, Player_Input.PlayerInput input);

    /// <summary>
    ///  Takes in a reference of the current state and the previous_states and the duration variable and the player input
    ///  changes the states based on the input
    ///  will modify the curr_state,prev_states and duration variable if state is changed
    /// </summary>
    /// <param name="curr_state">the current state </param>
    /// <param name="prev_states"> list of previous states </param>
    /// <param name="duration"> duration of the state </param>
    /// <returns>true if the state has been changed </returns>
    bool process_state(ref int curr_state, ref List<int> prev_states, ref float duration);


    
    void initialize(ref int curr_state, ref List<int> prev_states, ref float duration, ref List<Float_ref> cooldown_timers);
}


}