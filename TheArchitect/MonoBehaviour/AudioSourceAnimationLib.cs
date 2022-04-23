using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceAnimationLib : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_Clips;

    public void PlayAny()
    {
        if (this.m_Clips!=null && this.m_Clips.Length > 0)
        {
            Play(UnityEngine.Random.Range(0, this.m_Clips.Length));
        }
    }

    public void Play(int index)
    {
        AudioClip clip = this.m_Clips[index];
        
        GameObject tempAudioSource = new GameObject("TempAudio");
        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = 1;
        audioSource.spatialBlend = 0.0f;
        audioSource.Play();
        GameObject.Destroy(audioSource.gameObject, clip.length);
    }
}
