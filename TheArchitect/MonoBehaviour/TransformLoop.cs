using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLoop : MonoBehaviour
{
    [Header("Scale")]
    [SerializeField] private AnimationCurve m_ScaleCurve;
    [SerializeField] private Vector3 m_ScaleBase;
    [SerializeField] private Vector3 m_AffectScale;
    [Header("Rotation")]
    [SerializeField] private AnimationCurve m_RotationCurve;
    [SerializeField] private Vector3 m_RotationBase;
    [SerializeField] private Vector3 m_AffectRotation;
    [Header("Position")]
    [SerializeField] private AnimationCurve m_PositionCurve;
    [SerializeField] private Vector3 m_PositionBase;
    [SerializeField] private Vector3 m_AffectPosition;
    [Header("General")]
    [SerializeField] private bool m_UseStartTransform = true;
    [SerializeField] private bool m_ResetTransformOnUpdate = true;
    [SerializeField] private float m_Speed = 1;
    
    void Start()
    {
        if (this.m_UseStartTransform)
        {
            this.m_PositionBase = this.transform.position;
            this.m_RotationBase = this.transform.rotation.eulerAngles;
            this.m_ScaleBase = this.transform.localScale;
        }
    }

    void Update()
    {
        if (this.m_ResetTransformOnUpdate)
        {
            this.transform.localScale = this.m_ScaleBase;
            this.transform.rotation = Quaternion.Euler(this.m_RotationBase);
            this.transform.position = this.m_PositionBase;
        }
    }

    void LateUpdate()
    {
        if (this.m_AffectScale != Vector3.zero)
        {
            float v = m_ScaleCurve.Evaluate(Time.time * this.m_Speed);
            this.transform.localScale += (v * this.m_AffectScale);
        }
        if (this.m_AffectRotation != Vector3.zero)
        {
            float v = m_RotationCurve.Evaluate(Time.time * this.m_Speed);
            this.transform.rotation = Quaternion.Euler(this.m_RotationBase + (v * this.m_AffectRotation));
        }
        if (this.m_AffectPosition != Vector3.zero)
        {
            float v = m_PositionCurve.Evaluate(Time.time * this.m_Speed);
            this.transform.position = this.m_PositionBase + (v * this.m_AffectPosition);
        }
    }
}
