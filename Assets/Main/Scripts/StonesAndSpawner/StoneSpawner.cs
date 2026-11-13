using UnityEngine;
using UnityEngine.Pool;

namespace Main.Scripts
{
    public class StoneSpawner : MonoBehaviour
    {
        [SerializeField] GameObject Prefab;
        ObjectPool<GameObject> _pool;
        [SerializeField] float _wateTime = 3.0f;
        public bool CanSpawnStone = true;
        float _time;
        private void Awake()
        {
            _pool = new ObjectPool<GameObject>(
                createFunc: () => 
                {
                    var obj = Instantiate(Prefab);
                    obj.GetComponent<StoneBrain>().SetPull(_pool);
                    obj.GetComponent<StoneBrain>().spawner = this;
                    return obj;

                },
                actionOnGet: (obj) => 
                {
                    obj.SetActive(true);
                    obj.GetComponent<StoneBrain>().OnGet(transform);
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
            _time = Time.time;
        }
        
        void Update()
        {
        
            if (Time.time - _time > _wateTime && CanSpawnStone)
            {
                GameObject _polledStone = _pool.Get();
                _time = Time.time;
                CanSpawnStone = false;
            }
        }
    }
}

