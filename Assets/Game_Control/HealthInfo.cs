using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Util;
public class HealthInfo : MonoBehaviour
{
    public float max_health;
    public float curr_health;

    public bool is_invincible {get {return invincible_timer.Value > 0; }}
    private bool is_dead {get {return curr_health <= 0; }}

    private Float_ref invincible_timer;

    public void setInvincible(float duration){
        invincible_timer.Value = duration;
    }
    
    public void doDamage(int damage){
        if(!is_invincible && !is_dead){
            curr_health -= damage;
        }
    }

    void Start(){
        invincible_timer = new Float_ref(0);
    }
    void Update(){
        if(!is_invincible) return;
        invincible_timer.Value -= Time.deltaTime;
    }
    
}
