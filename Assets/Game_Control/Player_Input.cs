using System;

namespace Game_Control{

//TODO: implement this with information on the input
public static class Player_Input{
    
    [Flags]
    public enum Input{
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
    public static Input get_input(params Input[] inputs){
        Input input = Input.None;
        for (int i = 0; i < inputs.Length; i++)
        {
            input = input | inputs[i];            
        }
        return input;
    }



}


}