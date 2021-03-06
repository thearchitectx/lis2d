using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;
using TheArchitect.MonoBehaviour;

namespace TheArchitect.XMLScript.Action
{
    public class Choice
    {
        [XmlIgnore]
        public string Id;
        [XmlElement("check-flag", typeof(CheckFlag)),
            XmlElement("check-text", typeof(CheckText)),
            XmlElement("check-group", typeof(CheckGroupPredicate))]
        public Predicate[] Predicates;
        [XmlAttribute("pos")]
        public ChoicePos Pos = ChoicePos.NE;
        [XmlAttribute("out")]
        public string Output;
        [XmlAttribute("style")]
        public ChoiceStyle Style = ChoiceStyle.STANDARD;
        [XmlElement("text")]
        public string Text;
        [XmlElement("lock-reason")]
        public LockReason LockReason = null;
        // [XmlAttribute("icon")]
        // public ActionIcon Icon = ActionIcon.NONE;
        [XmlAttribute("icon-text")]
        public string IconText = null;
        [XmlElement("then")]
        public XMLScriptNode ThenNode;
    }

    public class LockReason
    {
        [XmlText]
        public string Text = null;
        [XmlElement("check-flag", typeof(CheckFlag)),
            XmlElement("check-text", typeof(CheckText)),
            XmlElement("check-group", typeof(CheckGroupPredicate))]
        public Predicate[] VisibilityPredicate;
    }

    public class ChoiceAction : XMLScriptAction
    {
        [XmlElement("c", typeof(Choice))]
        public Choice[] Choices = null;

        [XmlIgnore] private ChoiceWheel m_Wheel;
        [XmlIgnore] private string m_SelectedChoice;
        [XmlIgnore] private XMLScriptNode m_SelectedSubnode;
        [XmlIgnore] private Coroutine m_LoadCoroutine;
        [XmlIgnore] private string m_Output;

        public override void ResetState()
        {
            this.m_Wheel = null;
            this.m_SelectedChoice = null;
            this.m_SelectedSubnode = null;
            this.m_LoadCoroutine = null;
            this.m_Output = null;
        }

        public override string Update(XMLScriptInstance xmlscript, XMLScriptController controller)
        {
            if (this.m_LoadCoroutine == null)
                this.m_LoadCoroutine = controller.StartCoroutine(Load(controller));

            if (m_SelectedSubnode != null)
            {
                var output = m_SelectedSubnode.CurrentAction.Update(xmlscript, controller);
                if (output==OUTPUT_NEXT && m_SelectedSubnode.HasNextAction())
                {
                    m_SelectedSubnode.NextAction();
                    return null;
                }
                else
                {
                    return output;
                }
            }
            else if (this.m_SelectedChoice != null)
            {
                Addressables.ReleaseInstance(this.m_Wheel.gameObject);
                Choice c = FindChoice(this.m_SelectedChoice);

                if (c.ThenNode != null)
                {
                    this.m_SelectedSubnode = c.ThenNode;
                    this.m_SelectedSubnode.ResetState();
                    return null;
                }
                else {
                    return string.IsNullOrEmpty(c.Output) || c.Output.StartsWith("#") ? OUTPUT_NEXT : c.Output;
                }
            }
            
            return this.m_Output;
        }

        private System.Collections.IEnumerator Load(XMLScriptController controller)
        {
            foreach (var c in this.Choices)
                c.Id = System.Guid.NewGuid().ToString();

            var handle = Addressables.InstantiateAsync("choice-wheel.prefab", controller.transform);

            handle.Completed += (h) => {
                this.m_Wheel = handle.Result.GetComponent<ChoiceWheel>();

                int countChoicesVisible = 0;
                for (int i=0; i < Choices.Length; i++)
                {
                    Choice c = Choices[i];
                    bool condition = Predicate.Resolve(controller.Game, c.Predicates);
                    bool showLockReason = !condition && c.LockReason !=null && Predicate.Resolve(controller.Game, c.LockReason.VisibilityPredicate);
                    if ( showLockReason || condition)
                    {
                        countChoicesVisible++;
                        this.m_Wheel.AddChoice(
                            c.Id,
                            c.Pos,
                            c.Style,
                            condition ? ResourceString.Parse(c.Text, controller.Game.GetVariable) : ResourceString.Parse(c.LockReason.Text, controller.Game.GetVariable),
                            condition
                        );
                    }

                }

                if (countChoicesVisible==0) 
                {
                    Debug.LogWarning("No available choices!!");
                    Addressables.ReleaseInstance(this.m_Wheel.gameObject);
                    this.m_Output = OUTPUT_NEXT;
                }
                
                this.m_Wheel.onChoice.AddListener( choice => this.m_SelectedChoice = choice);
                this.m_Wheel.BuildChoices();
            };

            yield return handle;
        }

        public Choice FindChoice(string id)
        {
            foreach (var c in this.Choices)
            {
                if (c.Id == id)
                {
                    return c;
                }
            }

            return new Choice() { Output = OUTPUT_NEXT };
        }

    }
}