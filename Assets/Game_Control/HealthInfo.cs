using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Util;
public class HealthInfo : MonoBehaviour
{
    public float max_health;
    public float curr_health;

    public bool is_invincible {get {return invincible_timer.Value > 0; }}
    public bool is_dead {get {return curr_health <= 0; }}
    public bool parry_ready { get { return parry_ready_timer.Value > 0; } }
    public bool parry_bonus { get { return parry_bonus_timer.Value > 0; } }
    private bool parry_success = false;

    private Float_ref invincible_timer;
    private Float_ref parry_ready_timer;
    private Float_ref parry_bonus_timer;

    public void setInvincible(float duration){
        invincible_timer.Value = duration;
    }

    public void setParryReady(float duration)
    {
        parry_ready_timer.Value = duration;
    }

    public void setParryBonus(float duration)
    {
        parry_bonus_timer.Value = duration;
    }

    public void setParrySuccess(bool success)
    {
        parry_success = success;
    }

    public bool getParrySuccess()
    {
        return parry_success;
    }

    public void doDamage(int damage){
        if(!is_invincible && !is_dead){
            curr_health -= damage;
        }
    }

    void Start(){
        invincible_timer = new Float_ref(0);
        parry_ready_timer = new Float_ref(0);
        parry_bonus_timer = new Float_ref(0);
    }
    void Update(){
        if(invincible_timer.Value > 0)
        {
            invincible_timer.Value -= Time.deltaTime;
        }
        if (parry_ready_timer.Value > 0)
        {
            parry_ready_timer.Value -= Time.deltaTime;
        }
        if (parry_bonus_timer.Value > 0)
        {
            parry_bonus_timer.Value -= Time.deltaTime;
        }
    }
    
}
