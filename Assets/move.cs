using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 10.0f;
    public float JumpHeight = 1;
    private Rigidbody _rigibody;
    private Vector3 _playerInput;
    void Start()
    {
        _rigibody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _playerInput = new Vector3(Input.GetAxis("Horizontal"), 0,0);

        if (Input.GetButtonDown("Jump"))
        {
            _rigibody.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        move_character(_playerInput);
    }

    void move_character(Vector3 direction)
    {
        _rigibody.MovePosition(transform.position+(direction*speed*Time.deltaTime));
    }
}
