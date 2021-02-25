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
            RaycastHit hit;
            Vector3 direction;
            if (direction_right)
            {
                direction = transform.right;
            }
            else
            {
                direction = transform.right * -1;
            }
            if (!is_enemy && Physics.SphereCast(position, 1f, direction, out hit, 10, LayerMask.GetMask("EnemyHitbox")))
            {
                boss_health_info.doDamage(100);
                boss_health_info.setInvincible(0.3f);

            }
            else if (is_enemy && Physics.SphereCast(position, 1f, direction, out hit, 10, LayerMask.GetMask("PlayerHitbox")))
            {
                player_health_info.doDamage(100);
                boss_health_info.setInvincible(0.5f);
            }
        }


    }

}
