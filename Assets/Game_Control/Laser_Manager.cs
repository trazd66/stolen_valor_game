using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game_Control{

    public class Laser_Manager : MonoBehaviour
    {

        public HealthInfo player_health_info;
        public HealthInfo boss_health_info;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void fire_laser(Vector3 position, bool is_enemy, bool direction_right)

        {
            RaycastHit hit = new RaycastHit();
            Vector3 direction;
            bool successful_hit = false;
            //determine direction of laser fire
            if (direction_right)
            {
                direction = transform.right;
            }
            else
            {
                direction = transform.right * -1;
            }

            //check collision with enemies
            if (!is_enemy && Physics.SphereCast(position, 0.2f, direction, out hit, 10, LayerMask.GetMask("EnemyHitbox")))
            {
                
                boss_health_info.doDamage(100);
                boss_health_info.setInvincible(0.3f);
                successful_hit = true;

            }
            //check collision with player
            else if (is_enemy && Physics.SphereCast(position, 0.2f, direction, out hit, 10, LayerMask.GetMask("PlayerHitbox")))
            {
                if (player_health_info.parry_ready)
                {
                    player_health_info.setParrySuccess(true);
                }
                else
                {
                    player_health_info.doDamage(50);
                    boss_health_info.setInvincible(0.5f);
                }
                successful_hit = true;
            }
            //create laser visual
            GameObject laser_visual = new GameObject();
            laser_visual.transform.position = position;
            LineRenderer line_renderer = laser_visual.AddComponent<LineRenderer>();// A simple 2 color gradient with a fixed alpha of 1.0f.
            line_renderer.material = new Material(Shader.Find("Sprites/Default"));
            line_renderer.startColor = Color.red;
            line_renderer.endColor = Color.red;
            line_renderer.SetPosition(0, position);
            float distance;
            if (successful_hit)
            {
                distance = hit.distance;
            }
            else
            {
                distance = 10f;
            }
            if (direction_right)
            {
                line_renderer.SetPosition(1, new Vector3(position.x + distance, position.y, position.z));
            }
            else
            {
                line_renderer.SetPosition(1, new Vector3(position.x - distance, position.y, position.z));
            }

            line_renderer.widthMultiplier = 0.2f;
            Destroy(laser_visual, 0.15f);
        }


    }

}
