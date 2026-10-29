using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] public GameObject[] Hills;
    [SerializeField] private GameObject BoulderPrefab;
    [SerializeField] private GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameObject temphill = FindNearHill();
            Instantiate(BoulderPrefab, temphill.transform.position + new Vector3(0,100,0),temphill.transform.rotation);
        }
    }

    GameObject FindNearHill()
    {
        Vector3 playerposition = Player.transform.position;
        // s = sqr((x1 - x2)^2 + (y2 - y1)^2))
        GameObject temphill = null;
        float MinS = float.PositiveInfinity;
        foreach (GameObject hill in Hills)
        {
            float squareddistance = (playerposition - hill.transform.position).sqrMagnitude;
            if (squareddistance < MinS)// вычисление корня не требуется, так как нужно лишь сравнить расстояние
            {
                temphill = hill;
                MinS = squareddistance;
            }
        }
        return temphill;
    }
}
