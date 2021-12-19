using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheArchitect.SceneObjects;

public class MouseMoveAnimationControl : SceneObject
{
    public const string OUTCOME_START = "START";
    [SerializeField] private Animator m_Animator;
    [SerializeField] private string m_Layer;
    [SerializeField] private float m_Speed = 1;
    private int m_LayerIndex = 0;
    private float m_Pos = 0;

    void Start()
    {
        if (!string.IsNullOrEmpty(this.m_Layer))
            this.m_LayerIndex = this.m_Animator.GetLayerIndex(this.m_Layer);
    }

    public void ResetPos(){
        this.m_Pos = 0;
        this.SetOutcome(OUTCOME_START);
    }

    // Update is called once per frame
    void Update()
    {
        this.m_Pos += Input.GetAxis("Mouse Y") * this.m_Speed;

        this.m_Pos = Mathf.Clamp01(this.m_Pos);

        this.m_Animator.Play(0, this.m_LayerIndex, this.m_Pos);

        if (this.m_Pos == 1)
            this.SetOutcome(OUTCOME_END);
        else if (this.m_Pos == 0)
            this.SetOutcome(OUTCOME_START);
            
    }
}
