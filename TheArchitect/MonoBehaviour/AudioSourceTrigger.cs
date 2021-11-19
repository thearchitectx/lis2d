using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceTrigger : MonoBehaviour
{
    private AudioSource m_AudioSource;
    void Start()
    {
        this.m_AudioSource = this.GetComponent<AudioSource>();
    }

    public void PlayAudioSource()
    {
        this.m_AudioSource.Play();
    }
}
