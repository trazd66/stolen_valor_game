using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Game_Control{
    public class Player_controller : MonoBehaviour
    {

        int player_id;

        public float Speed = 5.0f;
        public float JumpHeight = 1.5f;


        private CharacterController _character_controller;
        // private Rigidbody rb;
        private Vector3 _velocity;
        
        public Collider[] AttackHitboxes;

        State_controller state_controller;
        State_controller attack_controller;

        public GameObject additional_part_0;
        private bool additional_part_0_toggle;

        public int attack_sequence_num = 0;
        public int attack_sequence_max = 5;
        // Start is called before the first frame update
        void Start()
        {
            _character_controller = GetComponent<CharacterController>();

            state_controller = new State_controller();
            state_controller.initialize(new Player_State_Transition_Func(_character_controller));

            attack_controller = new State_controller();
            attack_controller.initialize(new Attack_State_Transition_Func());

            

            }

        void FixedUpdate(){
        }

        // Update is called once per frame
        void Update()
        {
            // state_controller.process_state()
            // actually move the player 
            // every frame, we need to read plyaer input
            // and based on the player input, we transition states
            // and also move the player

            List<Player_Input.PlayerInput> inputs = new List<Player_Input.PlayerInput>();//TODO: remove this as it's costly


            if (Input.GetKeyDown(KeyCode.Q) && !additional_part_0_toggle){
                additional_part_0_toggle = true;
                GameObject obj = GameObject.Instantiate(additional_part_0,this.gameObject.transform);
                obj.transform.localPosition = new Vector3(0,0.7f,0);

            }
            //checked if left or right are inputed
            if (Input.GetAxis("Horizontal") != 0)
            {
                inputs.Add(Player_Input.PlayerInput.Dash);
            }

            //check if jump is inputed
            if (Input.GetButtonDown("Jump"))
            {
                inputs.Add((Player_Input.PlayerInput.Jump));
            }

            //check if attack is inputed
            if(Input.GetMouseButtonDown(0)){
                inputs.Add(Player_Input.PlayerInput.Attack);
            }

            //get a single PlayerInput to represent the totality of inputs
            Player_Input.PlayerInput total_input = Player_Input.get_input(inputs);

            //call the appropriate process_state
            bool processed = false;
            if (total_input == Player_Input.PlayerInput.None)
            {
                processed = state_controller.process_state();
            }
            else
            {
                processed = state_controller.process_state(total_input);
            }


            //apply movement to the player



            //stop downward movement when character lands
            if (_character_controller.isGrounded && _velocity.y < 0){
                _velocity.y = 0f;
            }

            //apply upwards momentum when jumping
            if (Input.GetButtonDown("Jump") && processed)
            {
                _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
            }

            //apply horizontal movement
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            _character_controller.Move(move * Time.deltaTime * Speed);

            // steering the character
            if (move != Vector3.zero)
                transform.right = move;

            //attack
            attack_controller.process_state(total_input);
            attack_controller.process_state();

            if(attack_controller.curr_state != (int) Attack_State_Transition_Func.attack_state.not_attacking){
                Collider col  = AttackHitboxes[0];
                 //check what Colliders on the PlayerHitbox layer overlap col
                Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("EnemyHitbox"));
                if(cols.Length > 0){
                    Debug.Log("hit");
                    cols[0].gameObject.GetComponentInParent<HealthInfo>().curr_health -= 10;
                }
            }
            
            //apply gravity
            _velocity.y += Physics.gravity.y * Time.deltaTime * 2;
            _character_controller.Move(_velocity * Time.deltaTime);

            //for debugging
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log(attack_controller.curr_state);
                Debug.Log(state_controller.curr_state);
            }

        }

        void OnCollisionEnter(Collision collision){

        }

    }


}
