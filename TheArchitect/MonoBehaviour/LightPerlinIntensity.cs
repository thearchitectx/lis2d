using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightPerlinIntensity : MonoBehaviour
{
    [SerializeField] private float m_StartNoiseX = 0;
    [SerializeField] private float m_StartNoiseY = 0;
    [SerializeField] private float m_Speed = 1;
    [SerializeField] private float m_Intensity = 1;
    [SerializeField] private float m_NoiseIntensity = 5;

    private float m_NoisePosition;
    private Light2D m_Light;

    void Start()
    {
        this.m_NoisePosition = this.m_StartNoiseX;
        this.m_Light = this.GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        this.m_NoisePosition += (Time.deltaTime * this.m_Speed);
        this.m_Light.intensity = 
            this.m_Intensity 
            + ( 
                Mathf.PerlinNoise( this.m_NoisePosition, this.m_StartNoiseY)
                * this.m_NoiseIntensity
            );
    }
}
