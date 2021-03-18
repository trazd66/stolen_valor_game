using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game_Control;
using Game_Util;

public class DamageCollider : MonoBehaviour
{
    // Start is called before the first frame update
    public Player_controller player_Controller;
    public Enemy1_controller enemy1_Controller;
    public ComboInfo combo_info;
    public HealthInfo player_health_info;
    public HealthInfo boss_health_info;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(this.tag == "Player_Attack_Hitbox"){
            handle_player_collider(collision);
        }else if(this.tag == "Enemy_Attack_Hitbox"){
            handle_boss_collider(collision);
        }

    }

    private void handle_player_collider(Collision collision){
        if(collision.collider.tag != "Enemy_Damage_Hitbox"){
            return;
        }

        int damage = 0;
        if(collision.contacts.Length > 0){
            ContactPoint contact = collision.contacts[0];
            switch (player_Controller.curr_atk_state)
            {
                case Attack_State_Transition_Func.attack_state.attack_basic_0:
                    damage = 30;
                    break;
                case Attack_State_Transition_Func.attack_state.attack_basic_1:
                    damage = 30;
                    break;
                case Attack_State_Transition_Func.attack_state.attack_basic_2:
                    damage = 50;
                    break;
                case Attack_State_Transition_Func.attack_state.attack_basic_3:
                    damage = 50;
                    break;
                case Attack_State_Transition_Func.attack_state.attack_basic_4:
                    damage = 50;
                    break;
                case Attack_State_Transition_Func.attack_state.attack_dash_0:
                    damage = 40;
                    break;
                case Attack_State_Transition_Func.attack_state.attack_jump_0:
                    damage = 40;
                    break;
                default:
                    break;
            }

            int cur_combo_points;                 
            if (damage > 0 && !boss_health_info.is_invincible)
            {
                Particle_system_controller.Instance.set_particle(CONTROL_CONFIG.VFX_HIT_PREFAB_NAME_1,contact.point,0.2f);

                if (player_health_info.parry_bonus)
                {
                    damage = (int)(damage * 1.5);
                }

                boss_health_info.doDamage(damage);
                Debug.Log(damage);
                AudioManager.instance.Play("game jam 3 impact");

                boss_health_info.setInvincible(0.4f);
                combo_info.setComboTimer(1.5f);
                    
                cur_combo_points = damage;                   
                float combo_multiplier = 1.0f + (combo_info.combo_counter * 0.1f);    
                
                if (combo_multiplier > 2.0f)                    
                {                        
                    combo_multiplier = 2.0f;                    
                }                    
                cur_combo_points = (int)(cur_combo_points * combo_multiplier);             
                
                combo_info.addComboPoints(cur_combo_points);
                combo_info.combo_counter++; 
            }
        }
    }

    private void handle_boss_collider(Collision collision){
        foreach (ContactPoint contact in collision.contacts)
        {

        }

    }
}
