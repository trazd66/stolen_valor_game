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


        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        _controller.Move(move * Time.deltaTime * Speed);
        _animator.SetFloat("walking_direction", move.x);
        
        _velocity.y += Physics.gravity.y * Time.deltaTime * 1.5f;
        _controller.Move(_velocity * Time.deltaTime);


        if (Input.GetButtonDown("Jump") )
        {
            _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
            
        }
        
        if (Input.GetAxis("Horizontal") != 0)
        {
            _animator.SetBool("iswalking", true);

            _animator.SetFloat("direction", Input.GetAxis("Horizontal"));

            if (Input.GetAxis("Horizontal") > 0)
            {
                _animator.SetFloat("walking_direction", 1);
            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                _animator.SetFloat("walking_direction", -1);
            }


        }


        if (Input.GetAxis("Horizontal") == 0)
        {
            _animator.SetBool("iswalking", false);
        }

        

        // steering the character










    }


    private bool isTurning()
    {

        return false;
    }

}
