using System;
using Main.Scripts;
using UnityEngine;
using UnityEngine.Pool;

public class StoneBrain : MonoBehaviour
{
    [SerializeField]
    public StoneSpawner spawner;
    private Rigidbody rb;
    private IObjectPool<GameObject> _pool;
    public bool canSpawnStone = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapone"))
        {
            canSpawnStone = true;
            spawner.time = Time.time;
        }
        
        
    }

    public void SetPull(ObjectPool<GameObject> pool)
    {
        _pool = pool;
    }
    
    public void OnGet(Transform _transform)
    {
        rb.useGravity = false;
        transform.position = _transform.position;
    }
    
    public void OnRelease()
    {
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
    }
    
}
