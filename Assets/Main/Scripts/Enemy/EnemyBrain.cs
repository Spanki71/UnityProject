
using System;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float speed;

    public EnemySpawner spawner;
    private Transform target;
    private ObjectPool<GameObject> pool;
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Stone")
        {
            animator.SetTrigger("Die");
            speed = 0;
        }
    }

    private void FixedUpdate()
    {
        Vector3 dir =  (target.position - transform.position).normalized;
        rb.linearVelocity = (dir * speed);
    }
    
    public void SetPull(ObjectPool<GameObject> pool)
    {
        this.pool = pool;
    }
    
    public void OnGet(Transform transform)
    {
        gameObject.transform.position = transform.position;
        target = spawner.target.transform;
        gameObject.SetActive(true);
        transform.position = transform.position;
    }
    
    public void OnRelease()
    {
        animator.ResetControllerState();
        gameObject.SetActive(false);
        rb.linearVelocity = Vector3.zero;
    }
}
