
using System;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    float speed;

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
        //rb.AddForce(dir * 300);
        rb.linearVelocity = (dir * speed);
    }
    
}
