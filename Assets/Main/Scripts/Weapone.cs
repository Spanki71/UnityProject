using UnityEngine;

namespace Main.Scripts
{
    public class Weapone : MonoBehaviour
    {
        public GameObject owner;
        
        [SerializeField]
        public float damage;
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == owner)
            {
                return;
            }
            else if (collision.gameObject.CompareTag("Stone"))
            {
                Vector3 hitpoint = collision.contacts[0].point;
                Vector3 dir = (collision.transform.position - hitpoint).normalized;
                collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
                collision.gameObject.GetComponent<Rigidbody>().AddForce(dir * damage, ForceMode.Impulse);
            }
        }
    }
}
