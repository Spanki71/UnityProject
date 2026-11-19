using System;
using System.Collections.Generic;
using Main.Scripts;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    public float wateInterval = 10.0f;
    [SerializeField]
    public float m_startWaveTime = 3.0f;
    [SerializeField]
    public float m_visibleTime = 5.0f;
    [SerializeField]
    private List<Transform> m_spawnPoints =  new List<Transform>(3);
    [SerializeField]
    private GameObject m_prefab;

    public event Action ToOnEnemyDieUpdateScore;
    public event Action WaveEndToUpdateScore;
    public GameObject target;
    public bool canSpawn = true;
    
    private float m_time;
    private ObjectPool<GameObject> m_pool;
    
    private WavesLogic m_wavesLogic;
    private int m_activeEnemyies;


    private void Awake()
    {
        m_activeEnemyies = 0;
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
        EnemyBrain brain = obj.GetComponent<EnemyBrain>();
        brain.OnEnemyDie += EnemyDie;
        brain.OnGet(m_spawnPoints[pos]);
        m_activeEnemyies++;
    }

    private void OnRelease(GameObject obj)
    {
        EnemyBrain brain = obj.GetComponent<EnemyBrain>();
        brain.OnEnemyDie -= EnemyDie;
        brain.ResetForPull();
        obj.SetActive(false);
    }

    void Start()
    {
        m_time = Time.time;
        m_wavesLogic.SetEnemySpawner(this);
        m_wavesLogic.StartWave(wateInterval,m_visibleTime, m_startWaveTime);
        m_wavesLogic.OnWaveEnd += WaveEnd;
    }
        
    void Update()
    {
        if (Time.time - m_time > wateInterval && canSpawn)
        {
            GameObject polledEnemy = m_pool.Get();
            m_time = Time.time;
        }
    }

    public void SetWaveLogic(WavesLogic logic)
    {
        m_wavesLogic = logic;
    }

    public int GetActiveEnemyies()
    {
        return m_activeEnemyies;
    }

    public void EnemyDie()
    {
        ToOnEnemyDieUpdateScore?.Invoke();
        m_activeEnemyies--;
        m_wavesLogic.CheckEndOfWave();
    }

    public void WaveEnd()
    {
        WaveEndToUpdateScore?.Invoke();
    }
}
