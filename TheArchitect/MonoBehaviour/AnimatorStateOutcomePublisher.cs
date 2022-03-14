using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheArchitect.SceneObjects;

public class AnimatorStateOutcomePublisher : StateMachineBehaviour
{
    [SerializeField] private string m_StateExitOutcome;
    [SerializeField] private string m_StateEnterOutcome;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SceneObject so = animator.GetComponent<SceneObject>();
        if (so != null && !string.IsNullOrEmpty(this.m_StateEnterOutcome))
            so.SetOutcome(this.m_StateEnterOutcome);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SceneObject so = animator.GetComponent<SceneObject>();
        if (so != null && !string.IsNullOrEmpty(this.m_StateExitOutcome))
            so.SetOutcome(this.m_StateExitOutcome);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
