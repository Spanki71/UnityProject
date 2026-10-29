using UnityEngine;

public class BoulderLifetimee : MonoBehaviour
{
    [SerializeField] GameObject Boulder;
    void Start()
    {
        Invoke("Delete",10f);
    }
    private void Delete()
    {
        Destroy(Boulder);
    }
}
