using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationTask : MonoBehaviour
{
    [SerializeField] GameObject hand;
    [SerializeField] GameObject[] Tools;
    [SerializeField] int DefaultTool;
    private GameObject CurrentTool;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        if (DefaultTool != 4)
        {
            CurrentTool = Instantiate(Tools[DefaultTool], hand.transform);
            CurrentTool.transform.SetParent(hand.transform);
        }
        animator.SetInteger("AnimationIndex", DefaultTool);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(CurrentTool);
            int i = Random.Range(0, Tools.Length);
            if (i != 4)
            {
                CurrentTool = Instantiate(Tools[i], hand.transform);
                CurrentTool.transform.SetParent(hand.transform);
            }
            animator.SetInteger("AnimationIndex", i);
        }
    }
}
