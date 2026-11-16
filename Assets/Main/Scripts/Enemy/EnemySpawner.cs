using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    public float wateTime = 20.0f;
    [SerializeField]
    private List<Transform> m_spawnPoints =  new List<Transform>(3);

    public GameObject target;
    
    private GameObject m_prefab;
    private float m_time;
    private ObjectPool<GameObject> m_pool;


    private void Awake()
    {
        m_prefab = transform.GetChild(0).gameObject;
        m_pool = new ObjectPool<GameObject>(
            createFunc: Spawn,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: 20,
            maxSize: 50
        );
    }

    private GameObject Spawn()
    {
        var obj = Instantiate(m_prefab);
        obj.GetComponent<EnemyBrain>().SetPull(m_pool);
        obj.GetComponent<EnemyBrain>().spawner = this;
        return obj;
    }

    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
        int pos = Random.Range(0, m_spawnPoints.Count);
        obj.GetComponent<EnemyBrain>().OnGet(m_spawnPoints[pos]);
    }

    private void OnRelease(GameObject obj)
    {
        obj.GetComponent<EnemyBrain>().OnRelease();
        obj.SetActive(false);
    }

    void Start()
    {
        m_time = Time.time;
    }
        
    void Update()
    {
        if (Time.time - m_time > wateTime)
        {
            GameObject polledEnemy = m_pool.Get();
            m_time = Time.time;
        }
    }
}
