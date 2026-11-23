using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicBrain : MonoBehaviour
{
    [SerializeField] public AudioClip clip;
    private AudioSource m_audioSource;

    private void Awake()
    {
        m_audioSource =  GetComponent<AudioSource>();
        
        m_audioSource.clip = clip;
        m_audioSource.loop = true;
        m_audioSource.playOnAwake = true;
        m_audioSource.Play();
    }
}
