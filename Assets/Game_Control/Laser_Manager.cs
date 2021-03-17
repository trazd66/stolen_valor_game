using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game_Control{

    public class Laser_Manager : MonoBehaviour
    {

        public HealthInfo player_health_info;
        public HealthInfo boss_health_info;

        public ComboInfo combo_info;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void aim_laser(Vector3 position, Vector3 direction)
        {
            //create laser visual
            GameObject laser_visual = new GameObject();
            laser_visual.transform.position = position;
            LineRenderer line_renderer = laser_visual.AddComponent<LineRenderer>();
            line_renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            //line_renderer.startColor = Color.gray;
            //line_renderer.endColor = Color.grey;
            line_renderer.material.SetColor("_Color", new Color(1f, 0f, 0f, 0.3f));

            //buncha code that allows transparency
            line_renderer.material.SetFloat("_Mode", 2);
            line_renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            line_renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            line_renderer.material.SetInt("_ZWrite", 0);
            line_renderer.material.DisableKeyword("_ALPHATEST_ON");
            line_renderer.material.EnableKeyword("_ALPHABLEND_ON");
            line_renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            line_renderer.material.renderQueue = 3000;




            line_renderer.SetPosition(0, position);

            line_renderer.SetPosition(1, direction);

            line_renderer.widthMultiplier = 0.2f;
            Destroy(laser_visual, 1.0f);
        }

        public void fire_laser(Vector3 position, bool is_enemy, Vector3 direction)

        {
            RaycastHit hit = new RaycastHit();
            Vector3 norm_direction = (new Vector3(direction.x - position.x, direction.y - position.y, direction.z - position.z)).normalized;
            bool successful_hit = false;
            //determine direction of laser fire

            if (!is_enemy && combo_info.canFireLaser())
            {
                combo_info.decreaseComboPoints(500f);
            }
            //not enough points to fire laser
            else if (!is_enemy)
            {
                return;
            }

            //check collision with enemies
            if (!is_enemy && Physics.SphereCast(position, 0.2f, norm_direction, out hit, 10, LayerMask.GetMask("EnemyHitbox")))
            {
                
                boss_health_info.doDamage(100);
                boss_health_info.setInvincible(0.3f);
                successful_hit = true;

            }
            //check collision with player
            else if (is_enemy && Physics.SphereCast(position, 0.2f, norm_direction, out hit, 10, LayerMask.GetMask("PlayerHitbox")))
            {
                if (player_health_info.parry_ready)
                {
                    player_health_info.setParrySuccess(true);
                }
                else
                {
                    player_health_info.doDamage(50);
                    player_health_info.setInvincible(0.5f);
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

            line_renderer.SetPosition(1, position + norm_direction * distance);

            line_renderer.widthMultiplier = 0.2f;
            Destroy(laser_visual, 0.15f);
        }


        public void laser_rain_aim(Vector3[] positions, Vector3[] directions)
        {
            for(int i = 0; i < positions.Length; i++)
            {
                //create laser visual
                GameObject laser_visual = new GameObject();
                laser_visual.transform.position = positions[i];
                LineRenderer line_renderer = laser_visual.AddComponent<LineRenderer>();
                line_renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
                //line_renderer.startColor = Color.gray;
                //line_renderer.endColor = Color.grey;
                line_renderer.material.SetColor("_Color", new Color(1f, 0f, 0f, 0.3f));

                //buncha code that allows transparency
                line_renderer.material.SetFloat("_Mode", 2);
                line_renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                line_renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                line_renderer.material.SetInt("_ZWrite", 0);
                line_renderer.material.DisableKeyword("_ALPHATEST_ON");
                line_renderer.material.EnableKeyword("_ALPHABLEND_ON");
                line_renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                line_renderer.material.renderQueue = 3000;




                line_renderer.SetPosition(0, positions[i]);
                line_renderer.SetPosition(1, directions[i]);

                line_renderer.widthMultiplier = 0.5f;
                Destroy(laser_visual, 1.50f);
            }
        }

        public void laser_rain_fire(Vector3[] positions, Vector3[] directions)
        {
            RaycastHit hit = new RaycastHit();
            bool successful_hit;
            for (int i = 0; i < positions.Length; i++)
            {
                successful_hit = false;
                Vector3 direction = (new Vector3(directions[i].x - positions[i].x, directions[i].y - positions[i].y, directions[i].z - positions[i].z)).normalized;
                if (Physics.SphereCast(positions[i], 0.4f, direction, out hit, 100, LayerMask.GetMask("PlayerHitbox")))
                {
                    if (player_health_info.parry_ready)
                    {
                        player_health_info.setParrySuccess(true);
                    }
                    else
                    {
                        player_health_info.doDamage(50);
                        player_health_info.setInvincible(0.5f);
                    }
                    successful_hit = true;
                }
                //create laser visual
                GameObject laser_visual = new GameObject();
                laser_visual.transform.position = positions[i];
                LineRenderer line_renderer = laser_visual.AddComponent<LineRenderer>();// A simple 2 color gradient with a fixed alpha of 1.0f.
                line_renderer.material = new Material(Shader.Find("Sprites/Default"));
                line_renderer.startColor = Color.red;
                line_renderer.endColor = Color.red;
                line_renderer.SetPosition(0, positions[i]);

                if (successful_hit)
                {
                    line_renderer.SetPosition(1, positions[i] + (direction * hit.distance));
                }
                else
                {
                    line_renderer.SetPosition(1, directions[i]);
                }

                line_renderer.widthMultiplier = 0.5f;
                Destroy(laser_visual, 0.15f);
            }
        }


    }

}
