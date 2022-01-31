using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TheArchitect.MonoBehaviour
{

    public enum ChoicePos
    {
        NONE = -1, NE = 0, SE = 1, SW = 2, NW = 3, E = 4, W = 5
    }

    public enum ChoiceStyle
    {
        STANDARD, PARAGON, RENEGADE, RIFT
    }

    public class ChoiceWheel : UnityEngine.MonoBehaviour
    {
        
        struct Choice
        {
            public string Id;
            public ChoicePos Pos;
            public ChoiceStyle style;
            public string Text;
            public bool Interactable;
        }

        public class ChoiceEvent : UnityEvent<string> { }

        [SerializeField] private Button[] m_Buttons;
        [SerializeField] private RuntimeAnimatorController m_AnimatorParagon;
        [SerializeField] private RuntimeAnimatorController m_AnimatorRenegade;
        [SerializeField] private RuntimeAnimatorController m_AnimatorRift;

        private List<Choice> m_Choices = new List<Choice>();
        public ChoiceEvent onChoice = new ChoiceEvent();

        public void AddChoice(string id, ChoicePos pos, ChoiceStyle style, string text, bool interactable)
        {
            this.m_Choices.Add(new Choice() { Id = id, Text = text, style = style, Pos = pos,  Interactable = interactable });
        }

        public void BuildChoices()
        {
            foreach (var b in this.m_Buttons)
                b.gameObject.SetActive(false);

            foreach (var c in this.m_Choices)
            {
                Button b = this.m_Buttons[(int) c.Pos];
                b.interactable = c.Interactable;
                b.GetComponentInChildren<Text>().text = c.Text;
                
                if (c.style == ChoiceStyle.PARAGON)
                    b.GetComponent<Animator>().runtimeAnimatorController = this.m_AnimatorParagon;
                if (c.style == ChoiceStyle.RENEGADE)
                    b.GetComponent<Animator>().runtimeAnimatorController = this.m_AnimatorRenegade;
                if (c.style == ChoiceStyle.RIFT)
                    b.GetComponent<Animator>().runtimeAnimatorController = this.m_AnimatorRift;

                b.onClick.AddListener(
                    () => {  if (Time.timeScale>0) onChoice.Invoke(c.Id); }
                );
                b.gameObject.SetActive(true);
            }
        }

        void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.U) && Input.GetKey(KeyCode.LeftControl))
                foreach (var b in this.GetComponentsInChildren<Button>())
                    b.interactable = true;
            #endif
        }
    }
}
