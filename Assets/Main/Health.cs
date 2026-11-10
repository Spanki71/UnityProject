using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public float health;
    private bool m_isAlive;
    
    public Animator animator;
    public void GetDamage(float damage)
    {
        animator.SetTrigger("GetDamage");
        if (health < damage)
        {
            health = 0.0f;
            m_isAlive = false;
            animator.SetTrigger("Die");
        }
        else
        {
            health -= damage;
        }
    }
    
}
