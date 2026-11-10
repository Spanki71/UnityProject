using Main.Interfaces;
using UnityEngine;

namespace Main.Scripts
{
    public class Weapone : MonoBehaviour
    {
        [SerializeField]
        public float damage;
        private void OnCollisionEnter(Collision collision)
        {
            collision.gameObject.GetComponentInParent<Health>()?.GetDamage(damage);
        }

        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.GetComponentInParent<Health>()?.GetDamage(damage);
        }
    }
}
