using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TheArchitect.MonoBehaviour
{
    struct Choice
    {
        public string Id;
        public string Text;
        public string Icon;
        public string IconText;
        public bool Interactable;
    }

    public class ChoiceEvent : UnityEvent<string> { }

    public class PanelChoice : UnityEngine.MonoBehaviour
    {
        [SerializeField] private Transform m_Help;
        private GameObject m_ButtonTemplate;
        private GameObject m_LastSelected;
        private List<Choice> m_Choices = new List<Choice>();
        public ChoiceEvent onChoice = new ChoiceEvent();
        public bool ShowHelpText = false;

        public void AddChoice(string id, string text, string icon, string iconText, bool interactable)
        {
            this.m_Choices.Add(new Choice() { Id = id, Text = text, Icon = icon, IconText = iconText, Interactable = interactable });
        }

        public void BuildChoices()
        {
            this.m_ButtonTemplate = this.transform.GetComponentInChildren<Button>().gameObject;

            if (!this.ShowHelpText)
                Destroy(this.m_Help.gameObject);

            Transform parent = this.transform.Find("panel");

            foreach (var c in this.m_Choices)
            {
                GameObject buttonObject = GameObject.Instantiate(this.m_ButtonTemplate);
                buttonObject.transform.SetParent(parent, false);
                buttonObject.GetComponentInChildren<Text>().text = c.Text;

                Button b = buttonObject.GetComponentInChildren<Button>();
                b.interactable = c.Interactable;
                b.onClick.AddListener(
                    () => {  if (Time.timeScale>0) onChoice.Invoke(c.Id); }
                );
                
                Image imageIcon = buttonObject.transform.Find("image-icon").GetComponent<Image>();
                // if (c.Icon != null && c.Icon != "")
                //     imageIcon.sprite = Resources.Load<Sprite>(c.Icon);
                imageIcon.gameObject.SetActive(imageIcon.sprite);
                Text textIcon = buttonObject.transform.Find("text-icon").GetComponent<Text>();
                if (!string.IsNullOrEmpty(c.IconText) && !string.IsNullOrEmpty(c.Icon))
                    textIcon.text = c.IconText;
                Image imageShadow = imageIcon.transform.Find("image-shadow").GetComponent<Image>();

                imageIcon.gameObject.SetActive(imageIcon.sprite);
                textIcon.gameObject.SetActive(!string.IsNullOrEmpty(c.IconText));
                imageShadow.gameObject.SetActive(!string.IsNullOrEmpty(c.IconText));

                // Fix padding for icon
                if (imageIcon.sprite!=null) 
                {
                    LayoutGroup layout = b.GetComponentInChildren<LayoutGroup>();
                    RectOffset padding = new RectOffset(layout.padding.left+32, layout.padding.right, layout.padding.top, layout.padding.bottom);
                    layout.padding = padding;
                }

            }

            Destroy(this.m_ButtonTemplate);
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
