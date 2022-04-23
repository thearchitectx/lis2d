using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationParticleEmitter : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_Particle;
    [SerializeField] private int m_PeaticlesPerSecondOnBurst;
    
    
    public void Burst(float seconds)
    {
        StartCoroutine(_Burst(seconds));
    }

    public void Emit(int count)
    {
        this.m_Particle.Emit( count );
    }

    private IEnumerator _Burst(float time)
    {
        float endTime = Time.time + time;
        while (Time.time < endTime) {
            this.m_Particle.Emit( (int) ( m_PeaticlesPerSecondOnBurst * Time.deltaTime ));
            yield return new WaitForSeconds(0.1f);
        }
    }
}
