using System;
using UnityEngine;

namespace TheArchitect.Utils
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorControl : UnityEngine.MonoBehaviour
    {
        public string BoolName1; 
        public bool BoolValue1; 

        public string IntName1; 
        public int IntValue1; 

        private Animator m_Animator;

        void Start()
        {
            this.m_Animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (!string.IsNullOrEmpty(BoolName1))
                this.m_Animator.SetBool(BoolName1, BoolValue1);
            if (!string.IsNullOrEmpty(IntName1))
                this.m_Animator.SetInteger(IntName1, IntValue1);

        }
    }
}

