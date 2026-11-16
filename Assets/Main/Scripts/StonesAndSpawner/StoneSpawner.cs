using UnityEngine;
using UnityEngine.Pool;

namespace Main.Scripts
{
    public class StoneSpawner : MonoBehaviour
    {
        [SerializeField] float wateTime = 5.0f;
        
        public bool CanSpawnStone = false;
        public float time;
        
        private GameObject Prefab;
        private ObjectPool<GameObject> m_pool;
        private GameObject m_activeStone;
        private StoneBrain m_activeStoneBrain;

        private void Awake()
        {
            Prefab = transform.GetChild(0).gameObject;
            m_pool = new ObjectPool<GameObject>(
                createFunc: () => 
                {
                    var obj = Instantiate(Prefab);
                    obj.GetComponent<StoneBrain>().SetPull(m_pool);
                    obj.GetComponent<StoneBrain>().spawner = this;
                    return obj;
                },
                actionOnGet: (obj) => 
                {
                    obj.SetActive(true);
                    m_activeStone = obj;
                    m_activeStoneBrain = obj.GetComponent<StoneBrain>();
                    m_activeStoneBrain.OnGet(transform);
                },
                actionOnRelease: (obj) =>
                {
                    obj.GetComponent<StoneBrain>().OnRelease();
                    obj.SetActive(false);
                },
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: 20,
                maxSize: 50
            );
        }
        
        void Start()
        {
            time = Time.time;
            GameObject _polledStone = m_pool.Get();
        }
        
        void Update()
        {
        
            if (m_activeStoneBrain.canSpawnStone && (Time.time - time > wateTime))
            {
                GameObject _polledStone = m_pool.Get();
                CanSpawnStone =  false;
            }
        }
    }
}

