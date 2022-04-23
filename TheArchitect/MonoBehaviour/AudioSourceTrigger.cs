using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_RandomClips;
    private AudioSource m_AudioSource;

    void Start()
    {
        this.m_AudioSource = this.GetComponent<AudioSource>();
    }

    public void PlayAudioSource()
    {
        if (this.m_RandomClips!=null && this.m_RandomClips.Length > 0)
        {
            this.m_AudioSource.clip = this.m_RandomClips[UnityEngine.Random.Range(0, this.m_RandomClips.Length)];
        }
        this.m_AudioSource.Play();
    }

    public void PlayAudioSourceFixed(int index)
    {
        if (this.m_RandomClips!=null && this.m_RandomClips.Length > index)
        {
            this.m_AudioSource.clip = this.m_RandomClips[index];
        }
        this.m_AudioSource.Play();
    }
}
