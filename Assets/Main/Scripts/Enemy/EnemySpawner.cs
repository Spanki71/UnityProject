using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Main.Scripts.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public event Action onEnemyDieUpdateScore;
        public event Action onWaveEndToUpdateScore;
        
        public GameObject target;
        private bool m_canSpawn = false;
        
        [SerializeField] public float waveTime = 10.0f;
        [SerializeField] public float m_startWaveTime = 65.0f;
        [SerializeField] public float m_visibleTime = 5.0f;
        
        [SerializeField] private List<Transform> m_spawnPoints =  new List<Transform>(3);
        [SerializeField] private GameObject m_prefab;
        
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

        public void SetWaveLogic(WavesLogic logic)
        {
            m_wavesLogic = logic;
        }

        public int GetActiveEnemyies()
        {
            return m_activeEnemyies;
        }

        public void OnEnemyDieHandler()
        {
            onEnemyDieUpdateScore?.Invoke();
            m_activeEnemyies--;
            m_wavesLogic.CheckEndOfWave();
        }

        public void OnWaveEndHandler()
        {
            onWaveEndToUpdateScore?.Invoke();
        }

        public void OnWaveStartHandler(PlayableDirector playableDirector)
        {
            m_wavesLogic.StartWaveAsync(0.0f,5.0f, m_startWaveTime);
        }

        public void SetCanSpawn(bool canSpawn)
        {
            m_canSpawn = canSpawn;
        }
        
        public bool GetCanSpawn()
        {
            return m_canSpawn;
        }
        
        void Start()
        {
            m_time = Time.time;
            m_wavesLogic.SetEnemySpawner(this);
            m_wavesLogic.onWaveEnd += OnWaveEndHandler;
        }
        
        void Update()
        {
            if (Time.time - m_time > waveTime && m_canSpawn)
            {
                GameObject polledEnemy = m_pool.Get();
                m_time = Time.time;
            }
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
            brain.onEnemyDie += OnEnemyDieHandler;
            brain.OnGet(m_spawnPoints[pos]);
            brain.SetSpeed(2.0f);
            m_activeEnemyies++;
        }

        private void OnRelease(GameObject obj)
        {
            EnemyBrain brain = obj.GetComponent<EnemyBrain>();
            brain.onEnemyDie -= OnEnemyDieHandler;
            brain.ResetForPull();
            obj.SetActive(false);
        }
    }
}
