using UnityEngine;

namespace Main.Scripts
{
    public class Shop : MonoBehaviour
    {
        [SerializeField]
        private GameObject pressE; 
        private void OnTriggerEnter(Collider other)
        {
            pressE.SetActive(true);
            Debug.Log(other.name);
        }

        private void OnTriggerExit(Collider other)
        {
            pressE.SetActive(false);
            Debug.Log(other.name);
        }
    }
}
