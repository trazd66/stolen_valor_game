using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculate_hit : MonoBehaviour
{
    // Start is called before the first frame update

    public Rigidbody player_rb;
    public Rigidbody enemy_rb;

    public int player_health = 100;
    public int enemy_health = 100;
    
    bool collide = false;
    void Start()
    {

    }

    void FixedUpdate(){
        if(collide){
            enemy_health -= 10;
        }
        collide = false;

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.attachedRigidbody == enemy_rb && enemy_rb != null){
            collide = true;
        }
    }
}
