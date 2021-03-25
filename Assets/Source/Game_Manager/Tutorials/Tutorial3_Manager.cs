using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Control;
using UnityEngine.UI;

public class Tutorial3_Manager : MonoBehaviour
{
    int tutorial_state = 0;

    public GameObject page0;

    public GameObject background;

    public GameObject tutorial_ui;

    public Player_controller player_controller;

    public HealthInfo health_info;

    public Tutorial3_Reward reward;

    public Image parry_indicator;

    public Laser_Manager laser_manager;

    private float laser_timer = 3f;

    private Vector3 position;
    private Vector3 direction;

    private bool aimed = false;
    private bool parried = false;
    public bool tutorial_finished = false;


    // Start is called before the first frame update
    void Start()
    {
        parry_indicator.color = Color.red;

        position = new Vector3(10, 1, -0.558f);
        direction = new Vector3(-10, 1, -0.558f);
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorial_state == 0 && Input.GetButtonDown("Jump"))
        {
            page0.SetActive(false);
            background.SetActive(false);
            tutorial_ui.SetActive(true);
            tutorial_state = 1;
        }
        else if (tutorial_state == 1)
        {
            player_controller.enable_control = true;

            if (health_info.parry_bonus)
            {
                parry_indicator.color = Color.green;
                reward.placeReward(new Vector3(4, 2, -0.558f));
                parried = true;
            }

            if(!parried){
                laser_timer -= Time.deltaTime;

                if (laser_timer <= 1.25 && !aimed)
                {
                    laser_manager.aim_laser(position, direction, 0.5f, 30f);
                    aimed = true;
                }

                if (laser_timer <= 0)
                {
                    laser_manager.fire_tutorial_laser(position, direction, 0.5f, 30f);
                    laser_timer = 4f;
                    aimed = false;
                }
            }
            
        }

        if(tutorial_finished){
            Game_Manager.instance.setState(3);
        }
    }
}
