using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceAnimationLib : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_Clips;
    [SerializeField] private bool m_NSFW = false;
    private AudioSource m_Player;

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
        
        if (this.m_Player == null)
        {
            var g = new GameObject("AudioSourceAnimationLibPlayer");
            this.m_Player = g.AddComponent<AudioSource>();
        }
        else if (this.m_Player.isPlaying)
        {
            GameObject.Destroy(this.m_Player.gameObject, this.m_Player.clip.length - this.m_Player.time);
            var g = new GameObject("AudioSourceAnimationLibPlayer");
            this.m_Player = g.AddComponent<AudioSource>();
        }

        this.m_Player.clip = clip;
        
        if (this.m_NSFW)
            this.m_Player.volume = GameSettings.GetVolumeNFSW();
        else
            this.m_Player.volume = 1;

        this.m_Player.volume = 1;
        this.m_Player.spatialBlend = 0.0f;
        this.m_Player.Play();

    }

}
