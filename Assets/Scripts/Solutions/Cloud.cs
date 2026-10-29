using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Cloud : MonoBehaviour
{
    [SerializeField] GameObject rain_Cloud;
    [SerializeField] GameObject[] Characters;
    [SerializeField] VisualEffect rain;
    private int i = 0;
    private Vector3 offset;
    private bool CoroutineActive = false;
    private void Start()
    {
        offset = new Vector3(0,20,0);
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !CoroutineActive)
        {
            StartCoroutine(Move(8f));
            if (i == 6)
            {
                i = 0;
            }
            else 
            {
                ++i;
            }
        }
    }
    IEnumerator Move(float delay)
    {
        rain.Stop();
        CoroutineActive = true;
        float StartTime = Time.time;
        while ((Time.time - StartTime) < delay)
        {
            rain_Cloud.transform.position += ((Characters[i].transform.position + offset) - rain_Cloud.transform.position) * Mathf.Clamp01(Time.deltaTime * 0.8f);
            yield return new WaitForSeconds(0.01f);// увеличить, если нужно оптимизировать
        }
        rain_Cloud.transform.position = Characters[i].transform.position + offset;
        CoroutineActive = false;
        Debug.Log("CoroutineStopped");
        rain.Play();
        StopCoroutine(Move(delay));
    }
}
