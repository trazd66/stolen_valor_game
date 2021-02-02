using System;
using System.Collections.Generic;

namespace Game_Control{

//TODO: implement this with information on the input
public static class Player_Input{
    
    [Flags]
    public enum PlayerInput{
        None = 0,
        Walk = 256,
        Dash = 1,
        Jump = 2,
        Attack = 4,
        Special_attack_0 = 8,
        Special_attack_1 = 16,
        Special_attack_2 = 32,
        Dodge = 64,
        Parry = 128        
    }

    /// <summary>
    /// helper to turn on multiple input flags
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public static PlayerInput get_input(List<PlayerInput> inputs){
        PlayerInput input = PlayerInput.None;
        for (int i = 0; i < inputs.Count; i++)
        {
            input = input | inputs[i];            
        }
        return input;
    }



}


}