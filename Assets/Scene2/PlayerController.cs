using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public Camera cam;

    public float speed = 15;
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private PlayerInput input;
    [SerializeField]
    private Animator animator;
    
    //public Animator animator;
    private Vector2 m_moveInput;
    public void OnMove(InputValue value)
    {
        m_moveInput = value.Get<Vector2>();
        animator.SetFloat("AxisX", m_moveInput.x);
        animator.SetFloat("AxisY", m_moveInput.y);
    }

    void Awake()
    {
        //input = GetComponent<PlayerInput>();
        //controller = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (GetForward() * m_moveInput.x + GetRight() * m_moveInput.y).normalized;
        controller.Move(direction * speed * Time.deltaTime);
    }

    public Vector3 GetForward()
    {
        Vector3 forwad = cam.transform.forward;
        forwad.y = 0;
        return forwad;
    }

    public Vector3 GetRight()
    {
        Vector3 right = cam.transform.right;
        right.y = 0;
        return right;
    }
}
