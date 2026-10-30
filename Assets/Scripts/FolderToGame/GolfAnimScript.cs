using UnityEngine;

public class GolfAnimScript : MonoBehaviour
{
    Animator _animator;
	public void Awake()
	{
		_animator = GetComponent<Animator>();
	}
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger("Hit");
        }
        
    }
}
