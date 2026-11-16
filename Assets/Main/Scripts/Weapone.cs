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
                Vector3 dir = (collision.transform.position - hitpoint).normalized;
                dir += Vector3.up * 0.02f;
                collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
                collision.gameObject.GetComponent<Rigidbody>().AddForce(dir * damage, ForceMode.Impulse);
            }
        }
    }
}
