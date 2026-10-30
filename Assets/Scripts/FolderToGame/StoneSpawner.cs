using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class StoneSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject Prefab;
    ObjectPool<GameObject> _pool;
	[SerializeField] float _wateTime = 10.0f;
    float _time;
	private void Awake()
	{
        _pool = new ObjectPool<GameObject>(
            createFunc: () => 
            {
                var obj = Instantiate(Prefab);
                obj.GetComponent<StoneFromPull>().SetPull(_pool);
                return obj;

			},
            actionOnGet: (obj) => 
            {
                obj.SetActive(true);
                obj.GetComponent<StoneFromPull>().OnGet(transform);
                },
            actionOnRelease: (obj) =>
            {
                obj.GetComponent<StoneFromPull>().OnRelease();
                obj.SetActive(false);
                },
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: 2,
            maxSize: 5
            );
	}
	void Start()
    {
		_time = Time.time;
	}

    // Update is called once per frame
    void Update()
    {
        
        if (Time.time - _time > _wateTime)
        {
            GameObject _polledStone = _pool.Get();
            _time = Time.time;
        }
        
        
    }
}

