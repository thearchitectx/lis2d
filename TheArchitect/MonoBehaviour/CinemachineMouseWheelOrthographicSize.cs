using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineMouseWheelOrthographicSize : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera m_Camera;
    [SerializeField] private float m_MaxSize = 5;
    [SerializeField] private float m_MinSize = 4;
    [SerializeField] private float m_WheelSpeed = 0.25f;
    [SerializeField] private float m_LensSpeed = 3;

    public float m_Target;

    void SetToMinLens()
    {
        this.m_Target = this.m_MinSize;
    }

    void Start()
    {
        this.m_Target = this.m_Camera.m_Lens.OrthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        this.m_Target = Mathf.Clamp(this.m_Target - Input.mouseScrollDelta.y * this.m_WheelSpeed, this.m_MinSize, this.m_MaxSize);
        this.m_Camera.m_Lens.OrthographicSize = Mathf.MoveTowards(this.m_Camera.m_Lens.OrthographicSize, this.m_Target, Time.deltaTime * m_LensSpeed);
    }
}
