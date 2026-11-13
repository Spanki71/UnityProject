using System;
using Main.Interfaces;
using UnityEngine;

namespace Main.Scripts
{
    public class Health : MonoBehaviour
    {
        public event Action OnDeath;
        public float health;
        private bool m_isAlive = true;
        public Animator animator;

        public Health(float Hp)
        {
            health = Hp;
        }

        public void GetDamage(float damage)
        {

            animator.SetTrigger("GetDamage");
            if (health <= damage)
            {
                health = 0.0f;
                m_isAlive = false;
                animator.SetBool("isAlive", m_isAlive);
                OnDeath?.Invoke();
            }
            else
            {
                health -= damage;
            }
        }
    
    }
}
