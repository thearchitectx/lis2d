using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheArchitect.SceneObjects;

public class MouseMoveAnimationControl : SceneObject
{
    public const string OUTCOME_START = "START";
    public const string OUTCOME_TARGET_DISTANCE = "TARGET_DISTANCE";
    [SerializeField] private Animator m_Animator;
    [SerializeField] private string m_Layer;
    [SerializeField] private float m_Speed = 1;
    [SerializeField] private string m_Axis = "Mouse Y";
    [SerializeField] private string m_AxisAlternate;
    [SerializeField] private float m_StartPos = 0;
    [SerializeField] private bool m_EnableAuto = true;
    [SerializeField] private float m_AutoTimeMultiplier = 1;

    private float m_Auto = -1;
    private float m_Distance = 0;
    private float m_TargetDistance = 0;


    private int m_LayerIndex = 0;
    private float m_Pos = 0;

    public void Invert()
    {
        this.m_Speed = -this.m_Speed;
    }

    public void SetTargetDistance(float distance){
        this.m_TargetDistance = distance;
    }

    void Start()
    {
        this.m_Pos = this.m_StartPos;
        if (!string.IsNullOrEmpty(this.m_Layer))
            this.m_LayerIndex = this.m_Animator.GetLayerIndex(this.m_Layer);
    }

    public void SetLayer(string layer) {
        this.m_Layer = layer;
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
        if (Time.deltaTime == 0)
            return;

        if (this.m_EnableAuto && Input.GetMouseButtonDown(2))
            this.m_Auto = this.m_Auto >= 0 ? -1 : 0;

        if (!string.IsNullOrEmpty(this.m_Axis))
        {
            var oldPos = this.m_Pos;

            if (this.m_Auto >= 0)
            {
                this.m_Pos = Mathf.PingPong(Time.time * this.m_AutoTimeMultiplier, 1);
            }
            else
            {
                this.m_Pos += Input.GetAxis(this.m_Axis) * this.m_Speed;
                if (!string.IsNullOrEmpty(this.m_AxisAlternate))
                {
                    this.m_Pos += Input.GetAxis(this.m_AxisAlternate) * this.m_Speed;
                }
            }


            this.m_Pos = Mathf.Clamp01(this.m_Pos);

            this.m_Distance += Mathf.Abs(oldPos - this.m_Pos);

            this.m_Animator.Play(0, this.m_LayerIndex, this.m_Pos);

            if (this.m_TargetDistance > 0 && this.m_Distance > this.m_TargetDistance) 
            {
                this.SetOutcome(OUTCOME_TARGET_DISTANCE);
            }

            if (this.m_Pos == 1)
                this.SetOutcome(OUTCOME_END);
            else if (this.m_Pos == 0)
                this.SetOutcome(OUTCOME_START);

        }
    }
}
