using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public Camera cam;
    public float speed = 15;
    CharacterController controller;
    PlayerInput input;
    private Animator m_animator;
    
    private Vector2 m_MoveInput;
    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        m_MoveInput = value.Get<Vector2>();
        m_animator.SetFloat("AxisX", m_MoveInput.x);
        m_animator.SetFloat("AxisY", m_MoveInput.y);
        Debug.Log("Move");
    }
    

    // Update is called once per frame
    void Update()
    {
        Vector2 m_MoveInput = input.actions["MOVE"].ReadValue<Vector2>();
        Vector3 direction = (GetForward() * m_MoveInput.y + GetRight() * m_MoveInput.x).normalized;
        controller.Move(direction * speed * Time.deltaTime);
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
