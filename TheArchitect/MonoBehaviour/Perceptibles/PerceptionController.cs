using UnityEngine;
using UnityEngine.UI;
using TheArchitect.Core;

namespace TheArchitect.MonoBehaviour.Perceptibles
{
    [RequireComponent(typeof(Collider2D))]
    public class PerceptionController : UnityEngine.MonoBehaviour
    {
        [SerializeField] private GameObject m_MultiplePerceptibleChoicePrefab;
        [SerializeField] private PlayerController m_PlayerController;
        
        private Collider2D m_Collider;
        private ContactFilter2D m_Filter;
        private Perceptible[] m_PriorResults = new Perceptible[5];
        private Perceptible m_Activated;
        private Collider2D[] m_Results = new Collider2D[5];
        
        private GameObject m_Choice;

        void Start()
        {
            this.m_Collider = this.GetComponent<Collider2D>();
            this.m_Filter = new ContactFilter2D() { layerMask = LayerMask.GetMask("Perceptible"), useLayerMask = true, useTriggers = true };
        }

        void LateUpdate()
        {
            if (Time.deltaTime==0)
                return;
                
            if (this.m_Choice != null || (this.m_Activated != null && this.m_Activated.OnActivated.gameObject.activeInHierarchy))
                return;

            this.m_Activated = null;

            for (int i=0; i < this.m_PriorResults.Length; i++)
            {
                if (this.m_PriorResults[i]!=null) 
                    this.m_PriorResults[i].OnNoticed.gameObject.SetActive(false);

                this.m_PriorResults[i] = null;
            }

            int p = this.m_Collider.OverlapCollider(this.m_Filter, this.m_Results);

            int perceptibleCount = 0;
            for (int i=0; i < p; i++)
            {
                var perceptible = this.m_Results[i].GetComponent<Perceptible>();
                if (perceptible != null)
                {
                    if (perceptible.CrouchOnly == this.m_PlayerController.Crouching)
                    {
                        this.m_PriorResults[perceptibleCount] = perceptible;
                        perceptible.OnNoticed.gameObject.SetActive(true);
                        perceptibleCount++;
                    }
                }
            }

            if (Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0))
            {
                if (perceptibleCount == 1)
                {
                    this.m_Activated = this.m_PriorResults[0];
                    this.m_Activated.OnActivated.gameObject.SetActive(true);
                    this.m_Activated.OnNoticed.gameObject.SetActive(false);
                }
                else if (perceptibleCount > 1)
                {
                    this.m_PlayerController.SetInputActive(false);
                    this.m_Choice = GameObject.Instantiate(this.m_MultiplePerceptibleChoicePrefab);
                    var buttonTemplate = this.m_Choice.GetComponentInChildren<Button>();
                    var canvas = this.m_Choice.GetComponentInChildren<Canvas>();
                    for (int i=0; i < perceptibleCount; i++)
                    {
                        var perceptible = this.m_PriorResults[i];
                        perceptible.OnNoticed.gameObject.SetActive(false);
                        
                        var b = GameObject.Instantiate(buttonTemplate);
                        b.transform.SetParent(canvas.transform, false);
                        b.GetComponentInChildren<Text>().text = perceptible.Label;
                        b.onClick.AddListener( () => {
                            this.m_PlayerController.SetInputActive(true);
                            this.m_Activated = perceptible;
                            this.m_Activated.OnActivated.gameObject.SetActive(true);
                            this.m_Activated.OnNoticed.gameObject.SetActive(false);
                            Destroy(this.m_Choice);
                            this.m_Choice = null;
                        });
                    }
                    
                    Destroy(buttonTemplate.gameObject);
                }
            }
        }
    }

}
