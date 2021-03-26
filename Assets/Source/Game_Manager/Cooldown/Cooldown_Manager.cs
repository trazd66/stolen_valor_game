using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown_Manager : MonoBehaviour
{

    public bool laser_ready = false;

    public Image laser_indicator;

    // Start is called before the first frame update
    void Start()
    {
        laser_indicator.color = Color.gray;
    }

    // Update is called once per frame
    void Update()
    {

        if (laser_ready && laser_indicator.color == Color.gray)
        {
            laser_indicator.color = Color.red;
        }
        else if (!laser_ready && laser_indicator.color == Color.red)
        {
            laser_indicator.color = Color.gray;
        }
    }
}
