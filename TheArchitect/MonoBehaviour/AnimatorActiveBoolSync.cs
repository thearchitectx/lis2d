using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheArchitect.SceneObjects;

public class AnimatorActiveBoolSync : SceneObject
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private string m_Bool = null;

    public void ToggleActive()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }

    void OnEnable()
    {
        if (!string.IsNullOrEmpty(this.m_Bool))
            this.m_Animator.SetBool(this.m_Bool, true);
    }

    void OnDisable()
    {
        if (!string.IsNullOrEmpty(this.m_Bool))
            this.m_Animator.SetBool(this.m_Bool, false);
    }
}
