using UnityEngine;

namespace Main.Scripts
{
    public class Weapone : MonoBehaviour
    {
        [SerializeField]
        public float damage;
        private void OnCollisionEnter(Collision collision)
        {
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.GetDamage(damage);
                Debug.Log($"Нанесенный урон: {damage}\nговорящий файл: Main/Weapone");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Health enemyHealth = other.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.GetDamage(damage);
                Debug.Log($"Нанесенный урон: {damage}\nговорящий файл: Main/Weapone");
            }
        }
    }
}
