using System;
using UnityEngine;

namespace TheArchitect.Utils
{
    public class AnimationRandomizer : UnityEngine.MonoBehaviour
    {

        public float factor = 0.15f;

        void Start()
        {
            Animator animator = GetComponent<Animator>();
            if (animator!=null)
            {
                animator.speed = 1 + Mathf.Lerp(-factor, factor,  UnityEngine.Random.value);
            }

            Destroy(this, 1f);
        }
    }
}

