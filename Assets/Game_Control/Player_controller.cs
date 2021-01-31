using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game_Control{
public class Player_controller : MonoBehaviour
{

    int player_id;

    
    State_controller state_controller;
    // Start is called before the first frame update
    void Start()
    {
        state_controller = new State_controller();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}


}
