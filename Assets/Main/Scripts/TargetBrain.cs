using System;
using UnityEngine;

namespace Main.Scripts
{
    public class TargetBrain : MonoBehaviour
    {
        public event Action onGetDamage;
        public event Action onGameOver;
        
        public float health;
        public float damageToTarget = 10.0f;
        
        [SerializeField] private float m_maxHealth = 100.0f;
        
        public void Reset()
        {
            health = m_maxHealth;
        }
    
        public float GetMaxHealth()
        {
            return m_maxHealth;
        }
        
        public void Start()
        {
            health =  m_maxHealth;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                SubtractHealth();
            }
        }

        private void SubtractHealth()
        {
            if (health > damageToTarget)
            {
                health -= damageToTarget;
                onGetDamage?.Invoke();
            }
            else
            {
                health = 0;
                onGetDamage?.Invoke();
                onGameOver?.Invoke();
            }
        }
    }
}
