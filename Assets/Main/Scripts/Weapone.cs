using Main.Interfaces;
using UnityEngine;

namespace Main.Scripts
{
    public class Weapone : MonoBehaviour
    {
        [SerializeField]
        public float damage;

        public GameObject Owner;

        private void OnCollisionEnter(Collision collision)
        {
            
            if (collision.gameObject == Owner)
            {
                return;
            }
            else if (collision.gameObject.CompareTag("Stone"))
            {
                Vector3 hitpoint = collision.contacts[0].point;
                Vector3 dir = (hitpoint - collision.transform.position).normalized;
                collision.gameObject.GetComponent<Rigidbody>().AddForce(dir * damage, ForceMode.VelocityChange);
                Debug.Log($"hitpoint {hitpoint} stone pos {collision.gameObject.transform.position}");
            }
        

            collision.gameObject.GetComponentInParent<Health>()?.GetDamage(damage);
        }
    }
}
