using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TheArchitect.MonoBehaviour
{

    public enum ChoicePos
    {
        NE = 0, SE = 1, SW = 2, NW = 3, E = 4, W = 5
    }

    public class ChoiceWheel : UnityEngine.MonoBehaviour
    {
        
        struct Choice
        {
            public string Id;
            public ChoicePos Pos;
            public string Text;
            public bool Interactable;
        }

        public class ChoiceEvent : UnityEvent<string> { }

        [SerializeField] private Button[] m_Buttons;

        private List<Choice> m_Choices = new List<Choice>();
        public ChoiceEvent onChoice = new ChoiceEvent();

        public void AddChoice(string id, ChoicePos pos, string text, bool interactable)
        {
            this.m_Choices.Add(new Choice() { Id = id, Text = text, Pos = pos,  Interactable = interactable });
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
