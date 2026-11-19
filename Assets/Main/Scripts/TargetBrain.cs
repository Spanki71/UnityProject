using System;
using UnityEngine;

public class TargetBrain : MonoBehaviour
{
    [SerializeField]
    private float m_maxHealth = 100.0f;

    public float health;

    public event Action OnGetDamage;
    public event Action OnGameOver;
    public float damageToTarget = 10.0f;

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
            OnGetDamage?.Invoke();
        }
        else
        {
            health = 0;
            OnGetDamage?.Invoke();
            OnGameOver?.Invoke();
        }
    }

    public void Reset()
    {
        health = m_maxHealth;
    }
    
    public float GetMaxHealth()
    {
        return m_maxHealth;
    }
}
