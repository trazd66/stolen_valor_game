using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Util;

public class ComboInfo : MonoBehaviour
{

    public float combo_counter = 0;
    private float combo_points = 0;

    private static float laser_cost = 500;

    public bool combo_timer_active { get { return combo_timer.Value > 0; } }

    private Float_ref combo_timer;

    // Start is called before the first frame update
    void Start()
    {
        combo_timer = new Float_ref(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(combo_timer.Value > 0)
        {
            combo_timer.Value -= Time.deltaTime;
            if(combo_timer.Value <= 0)
            {
                combo_counter = 0;
            }
        }
    }

    public void setComboTimer(float duration)
    {
        combo_timer.Value = duration;
    }

    public void addComboPoints(float amount)
    {
        combo_points += amount;
        if (combo_points > 2000f)
        {
            combo_points = 2000f;
        }
    }

    public void decreaseComboPoints(float amount)
    {
        combo_points -= amount;
        if (combo_points < 0f)
        {
            combo_points = 0f;
        }
    }

    public float getComboPoints()
    {
        return combo_points;
    }

    public bool canFireLaser()
    {
        return combo_points >= laser_cost;
    }
}
