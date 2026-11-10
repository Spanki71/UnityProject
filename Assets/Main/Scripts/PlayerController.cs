using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Scripts
{
    public class PlayerController
    {
        public event Action smt;
        public event Action OnDie;
        private const int Mass = 20;
        [SerializeField]
        public Camera cam;
        [SerializeField]
        private float baseSpeed = 15;
        [SerializeField]
        private Transform cameraPivot;
        [SerializeField]
        public float jumpForce;
    
        public float speed;
        CharacterController controller;
        PlayerInput input;
        private Animator m_animator;

        private Vector2 m_moveInput;
        private Vector2 m_lookInput;

        private float verticalForce;


        private float pitch = 0;
        private void Awake()
        {
            //input = GetComponent<PlayerInput>();
            //controller = GetComponent<CharacterController>();
            //m_animator = GetComponent<Animator>();
        }

        public void OnAttack()
        {
            m_animator.SetTrigger("Attack");
        }

        public void OnMove(InputValue value)
        {
            m_moveInput = value.Get<Vector2>();
            m_animator.SetFloat("AxisX", m_moveInput.x);
            m_animator.SetFloat("AxisY", m_moveInput.y);
        }

        public void OnJump(InputValue value)
        {
            if (controller.isGrounded)
            {
                verticalForce = jumpForce;
            }
        }

        public void OnLook(InputValue value)
        {
            m_lookInput = value.Get<Vector2>();
            // !!! Важная логика в комментарии  !!!!
        
            //gameObject.transform.Rotate(Vector3.up, m_lookInput.x);
        
            float newPitch = Mathf.Clamp(pitch + m_lookInput.y, -90f, 90f);
            float delta = pitch - newPitch;
            pitch = newPitch;
            cameraPivot.Rotate(Vector3.right, delta);
            //cam.transform.RotateAround(cameraPivot.position, cam.transform.right, delta);
        }

        // Update is called once per frame
        void Update()
        {
            verticalForce -= 9.81f * Mass *  Time.deltaTime;
            if (controller.isGrounded && verticalForce < 0.0f)
            {
                verticalForce = -2.0f;
            }
        
            Vector3 direction = (GetForward() * m_moveInput.y + GetRight() * m_moveInput.x).normalized;
            Vector3 verticalDirection = Vector3.up * (verticalForce * Time.deltaTime);
        
            controller.Move(direction * (baseSpeed * Time.deltaTime) + verticalDirection);
        
            speed = controller.velocity.magnitude;
            m_animator.SetFloat("Speed", speed);
        
        
        }

        public Vector3 GetForward()
        {
            Vector3 forward = cam.transform.forward;
            forward.y = 0;
            return forward;
        }

        public Vector3 GetRight()
        {
            Vector3 right = cam.transform.right;
            right.y = 0;
            return right;
        }
    }
}
