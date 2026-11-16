using Main.Scripts;
using UnityEngine;
using UnityEngine.Pool;

public class StoneBrain : MonoBehaviour
{
    [SerializeField]
    public StoneSpawner spawner;
    
    public bool canSpawnStone = false;
    private Rigidbody rb;
    private IObjectPool<GameObject> m_pool;
    private bool m_used;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!m_used)
        {
            canSpawnStone = true;
            spawner.time = Time.time;
            m_used = true;
        }

        if (!collision.gameObject.CompareTag("Weapone"))
        {
            Invoke("GoPull", 10.0f);
        }
    }

    public void GoPull()
    {
        m_pool.Release(gameObject);
    }

    public void SetPull(ObjectPool<GameObject> pool)
    {
        m_pool = pool;
    }
    
    public void OnGet(Transform _transform)
    {
        m_used = false;
        canSpawnStone = false;
        rb.useGravity = false;
        transform.position = _transform.position;
    }
    
    public void OnRelease()
    { 
        canSpawnStone = false;
        CancelInvoke(nameof(GoPull));
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
    }
    
}
