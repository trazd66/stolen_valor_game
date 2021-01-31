using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class movement : MonoBehaviour
{
    //define the speed and other component
    public float Speed = 10.0f;
    public float JumpHeight = 2f;


    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _attacking = false;
    private float _attackTimer = 0f;

    public GameObject[] AttackObjects;
    private List<int> _attackIndices;

    public Collider[] AttackHitboxes;
    
    // Start is called before the first frame update
    void Start()
    {
        // get the controller
        _controller = GetComponent<CharacterController>();
        _attackIndices = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {

        if (_attackTimer <= 0 && _attacking)
        {
            _attackTimer = 0f;
            _attacking = false;

            //remove all attack visuals
            for(int i = _attackIndices.Count - 1; i >= 0; i--)
            {
                AttackObjects[_attackIndices[i]].SetActive(false);
                _attackIndices.RemoveAt(i);
            }
        }

        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }


        //move position
        if (_controller.isGrounded && _velocity.y < 0)
            _velocity.y = 0f;

        if (Input.GetButtonDown("Jump") )
        {
            _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
            Debug.Log(Input.GetButtonDown("Jump"));
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        _controller.Move(move * Time.deltaTime * Speed);


        

        // steering the character
        if (move != Vector3.zero)
            transform.right = move;

        _velocity.y += Physics.gravity.y * Time.deltaTime*2;
        _controller.Move(_velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.F) && !_attacking)
        {
            _attacking = true;
            AttackObjects[0].SetActive(true);
            LaunchAttack(AttackHitboxes[0]);
            _attackIndices.Add(0);
            _attackTimer = 0.5f;
        } 

    }



    //called when an attack is launched
    private void LaunchAttack(Collider col)
    {
        //check what Colliders on the EnemyHurtbox layer overlap col
        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("EnemyHurtbox"));
        
        foreach(Collider c in cols)
        {
            float damage = 0;

            //add damage based on what's attacking
            switch (col.name)
            {
                case "BasicAttack":
                    damage += 20;
                    break;
                default:
                    Debug.Log("Unable to identify attack, make sure switch case matches.");
                    break;
            }

            //add damage based on what's being hit
            switch (c.name)
            {
                case "Obstacle":
                    damage += 10;
                    break;
                default:
                    Debug.Log("Unable to identify target, make sure switch case matches.");
                    break;
            }

            Debug.Log(damage);
        }

    }
}
