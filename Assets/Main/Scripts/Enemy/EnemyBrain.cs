
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
    private float speed = 2.0f;

    public EnemySpawner spawner;
    private Transform target;
    private ObjectPool<GameObject> pool;
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stone"))
        {
            if (collision.impulse.magnitude > 1f)
            {
                animator.SetTrigger("Die");
                speed = 0;
                Invoke("GoPull", 2f);
            }
        }

        if (collision.gameObject.CompareTag("Target"))
        {
            GoPull();
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

    public void GoPull()
    {
        pool.Release(gameObject);
    }

    public void ResetForPull()
    {
        animator.ResetControllerState();
        rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }

}
