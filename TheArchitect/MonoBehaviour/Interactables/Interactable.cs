using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TheArchitect.SceneObjects;

namespace TheArchitect.MonoBehaviour.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class Interactable : UnityEngine.MonoBehaviour
    {
        [SerializeField] private Transform m_OnMouseOver;
        [SerializeField] private SceneObject m_SceneObject;
        [SerializeField] private string m_Label;
        [SerializeField] private string m_Outcome;
        

        void OnEnable()
        {
            if (this.m_SceneObject == null && this.transform.parent != null)
            {
                this.m_SceneObject =this.transform.parent.GetComponent<SceneObject>();
            }

            if (this.m_SceneObject == null)
                Debug.LogWarning("m_SceneObject not present");

            var t = this.m_OnMouseOver.GetComponentInChildren<Text>(true);
            if (t!=null)
                t.text = this.m_Label;

            this.m_OnMouseOver.gameObject.SetActive(false);
        }

        void OnMouseEnter()
        {
            if (this.m_OnMouseOver != null)
                this.m_OnMouseOver.gameObject.SetActive(true);
        }

        void OnMouseExit()
        {
            if (this.m_OnMouseOver != null)
                this.m_OnMouseOver.gameObject.SetActive(false);
        }

        void OnMouseUpAsButton()
        {
            if (this.m_SceneObject != null)
            {
                this.m_SceneObject.SetOutcome(this.m_Outcome);
            }
        }
    }
}