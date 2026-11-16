using System;
using UnityEngine;

public class TargetBrain : MonoBehaviour
{
    [SerializeField]
    public float health = 100.0f;

    public event Action OnGameOver;
    public float damageToTarget = 10.0f;

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
        }
        else
        {
            health = 0;
            OnGameOver?.Invoke();
        }
    }
}
