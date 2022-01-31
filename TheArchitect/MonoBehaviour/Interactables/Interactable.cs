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
        [SerializeField] private Collider2D m_Collider;
        [SerializeField] private string m_Label;
        [SerializeField] private string m_Outcome;

        void OnEnable()
        {
            this.m_Collider = this.GetComponent<Collider2D>();

            if (this.m_SceneObject == null && this.transform.parent != null)
            {
                this.m_SceneObject =this.transform.parent.GetComponent<SceneObject>();
            }

            if (this.m_SceneObject == null)
                Debug.LogWarning("m_SceneObject not present");

            var t = this.m_OnMouseOver.GetComponentInChildren<Text>(true);
            if (t!=null)
                t.text = this.m_Label;

        }

        void Update()
        {
            if (Time.deltaTime == 0)
                return;

            var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(this.m_Collider.OverlapPoint(point))
            {
                CursorManager.RequestTargetHand();
                this.m_OnMouseOver.gameObject.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    StopAllCoroutines();
                    StartCoroutine(this.DelayedOutcome());
                }
            }
            else
            {
                this.m_OnMouseOver.gameObject.SetActive(false);
            }
        }

        private System.Collections.IEnumerator DelayedOutcome()
        {
            yield return new WaitForSeconds(0.25f);
            if (this.m_SceneObject.Outcome != "LEAVE")
                this.m_SceneObject.SetOutcome(this.m_Outcome);
        }

    }
}