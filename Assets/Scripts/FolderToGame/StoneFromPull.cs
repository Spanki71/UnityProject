using UnityEngine;
using UnityEngine.Pool;

public class StoneFromPull : MonoBehaviour
{
    IObjectPool<GameObject> _pool;
    float LifeTime = 1.0f;
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
        CancelInvoke("ReturnToPool");
    }
    public void ReturnToPool()
    {
        _pool.Release(gameObject);
    }
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
