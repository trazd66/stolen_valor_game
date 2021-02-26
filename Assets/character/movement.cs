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
    private Animator _animator;
    private int moving_Dirl;
    private bool walking;
    private float lerpSpeed;
    private float lerprot;
    // Start is called before the first frame update
    void Start()
    {
        // get the controller
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _animator.enabled = true;
        moving_Dirl = 0;
        walking = false; 
    }

    // Update is called once per frame
    void Update()
    {
        //move position
        if (_controller.isGrounded && _velocity.y < 0)
            _velocity.y = 0f;

        if (Input.GetButtonDown("Jump"))
        {
            _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
            Debug.Log(Input.GetButtonDown("Jump"));
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        _controller.Move(move * Time.deltaTime * Speed);
        
        lerpSpeed = 20 * Time.deltaTime;


        // steering the character
        lerprot = move.x;
        Quaternion right_rot = Quaternion.Euler(0, 0, 0);
        Quaternion left_rot = Quaternion.Euler(0, -180, 0);
        if (move.x>0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, right_rot, lerpSpeed);
        }

        if (move.x <0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, left_rot, lerpSpeed);
        }
        Debug.Log(transform.right.x);




        _velocity.y += Physics.gravity.y * Time.deltaTime * 2;
        _controller.Move(_velocity * Time.deltaTime);
         


        // steering the character










    }


    private bool isTurning()
    {

        return false;
    }

}
