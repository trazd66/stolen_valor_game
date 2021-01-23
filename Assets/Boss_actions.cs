using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_actions : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Transform _transform;
    private const float _speed = 0.3f;
    private float _direction = 0;
    private bool moving = false;
    private Vector3 _jumpFactor = new Vector3(0, 20, 0);
    private bool _jumped;
    public GameObject sound;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _direction = 0;
        _jumped = false;
        sound = GameObject.Find("Sound");
        source = GetComponent<AudioSource>();
        move_left();

    }

    // Update is called once per frame
    void Update()
    {
        if (_transform.position.x >= 4 && !moving)
        {
            move_left();
            moving = true;
        }
        if (_transform.position.x <= -4 && !moving)
        {
            move_right();
            moving = true;
        }

        if (_transform.position.x <= -4 || _transform.position.x >= -4)
        {
            
            moving = false;
        }


    }

    private void FixedUpdate()
    {
        _rigidbody.velocity += _transform.forward * _speed * _direction;
        if (_jumped)
        {
            _rigidbody.AddForce(Vector3.up*4, ForceMode.VelocityChange);
            _jumped = false;
        }
    }

    void move_left()
    {
        _direction = 1;
    }
    void move_right()
    {
        _direction = -1;
    }
    void attacked()
    {
        if (_direction == -1)
        {
            _direction = 1;
        }
        else
        {
            _direction = -1;
        }
        source.Play();
        _jumped = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            
            attacked();
        }
    }
    
    

}
