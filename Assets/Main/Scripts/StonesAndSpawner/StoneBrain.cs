using System;
using Main.Scripts;
using UnityEngine;
using UnityEngine.Pool;

public class StoneBrain : MonoBehaviour
{
    public StoneSpawner spawner;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        spawner.CanSpawnStone = true;
        Debug.Log("Can Spawn Stone");
    }
    
    
    IObjectPool<GameObject> _pool;
    [SerializeField] float LifeTime = 30.0f;
    public void SetPull(ObjectPool<GameObject> pool)
    {
        _pool = pool;
    }
    public void OnGet(Transform _transform)
    {
        CancelInvoke("ReturnToPool");
        transform.position = _transform.position;
        Invoke("ReturnToPool", LifeTime);
    }
    public void OnRelease()
    {
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        CancelInvoke("ReturnToPool");
    }
    public void ReturnToPool()
    {
        _pool.Release(gameObject);
    }
    
    
    
}
