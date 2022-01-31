using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheArchitect.SceneObjects;
using UnityEngine.Animations;

public class MouseMoveConstraintControl : SceneObject
{
    public const string OUTCOME_TARGET_DISTANCE = "TARGET_DISTANCE";
    public const string OUTCOME_TIMER = "TIMER";
    [SerializeField] private ParentConstraint[] m_Constraints;
    [SerializeField] private float m_Speed = 1;
    [SerializeField] private string m_Axis = "Mouse Y";
    [SerializeField] private float m_StartPos = 0;
    private float m_Pos = 0;
    private float m_Distance = 0;
    private float m_TargetDistance = 0;
    private float m_TimeToTimerOutcome = 0;


    void Start()
    {
        this.m_Pos = this.m_StartPos;
    }

    public void ResetPos() {
        this.m_Pos = 0;
    }

    public void SetTargetDistance(float distance){
        this.m_TargetDistance = distance;
    }

    public void StartTimer(float duration) {
        this.m_TimeToTimerOutcome = Time.time + duration;
    }

    // Update is called once per frame
    void Update()
    {
        var oldPos = this.m_Pos;
        this.m_Pos += Input.GetAxis(this.m_Axis) * this.m_Speed;

        this.m_Pos = Mathf.Clamp01(this.m_Pos);

        this.m_Distance += Mathf.Abs(oldPos - this.m_Pos);

        foreach (var c in this.m_Constraints)
            c.weight = this.m_Pos;

        if (this.m_TimeToTimerOutcome > 0 && Time.time > this.m_TimeToTimerOutcome)
        {
            this.SetOutcome(OUTCOME_TIMER);
        }
        else if (this.m_TargetDistance > 0 && this.m_Distance > this.m_TargetDistance) 
        {
            this.SetOutcome(OUTCOME_TARGET_DISTANCE);
        }
            
    }
}
