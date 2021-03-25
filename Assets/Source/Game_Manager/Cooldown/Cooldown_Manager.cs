using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown_Manager : MonoBehaviour
{

    public bool dodge_ready = false;
    public bool laser_ready = false;

    public Image dodge_indicator;
    public Image laser_indicator;

    // Start is called before the first frame update
    void Start()
    {
        dodge_indicator.color = Color.red;
        laser_indicator.color = Color.gray;
    }

    // Update is called once per frame
    void Update()
    {
        if (dodge_ready && dodge_indicator.color == Color.red)
        {
            dodge_indicator.color = Color.green;
        }
        else if (!dodge_ready && dodge_indicator.color == Color.green)
        {
            dodge_indicator.color = Color.red;
        }

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
