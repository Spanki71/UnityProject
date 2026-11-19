
using System;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField]
    private Animator m_animator;
    [SerializeField]
    private Rigidbody m_rb;
    [SerializeField]
    private float m_speed = 2.0f;
    
    public event Action OnEnemyDie;
    public EnemySpawner spawner;
    
    private Transform m_target;
    private ObjectPool<GameObject> m_pool;
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stone"))
        {
            if (collision.impulse.magnitude > 1f)
            {
                OnEnemyDie?.Invoke();
                m_animator.SetTrigger("Die");
                m_speed = 0;
                Invoke("GoPull", 2f);
            }
        }

        if (collision.gameObject.CompareTag("Target"))
        {
            OnEnemyDie?.Invoke();
            GoPull();
        } 
    }

    private void FixedUpdate()
    {
        Vector3 dir =  (m_target.position - transform.position).normalized;
        m_rb.linearVelocity = (dir * m_speed);
        if (dir.magnitude > 0.1f)
        {
            gameObject.transform.rotation = Quaternion.LookRotation(dir);
        }
    }
    
    public void SetPull(ObjectPool<GameObject> pool)
    {
        this.m_pool = pool;
    }
    
    public void OnGet(Transform transform)
    {
        gameObject.transform.position = transform.position;
        m_target = spawner.target.transform;
        gameObject.SetActive(true);
        transform.position = transform.position;
    }

    public void GoPull()
    {
        m_pool.Release(gameObject);
    }

    public void ResetForPull()
    {
        m_animator.ResetControllerState();
        m_rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }

}
