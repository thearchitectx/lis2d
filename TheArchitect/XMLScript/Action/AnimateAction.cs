using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class AnimateAction : XMLScriptAction
    {
        [XmlAttribute("trigger")]
        public string Trigger = null;
        [XmlAttribute("target")]
        public string Target = null;
        [XmlAttribute("idle")]
        public BehaviourAlias Idle = BehaviourAlias.NONE;
        [XmlAttribute("face")]
        public BehaviourAlias Face = BehaviourAlias.NONE;
        [XmlAttribute("waitEndOfClip")]
        public int waitEndOfClip = -1;
        [XmlElement("bool")]
        public AnimBool[] Booleans = null;
        [XmlElement("int")]
        public AnimInt[] Integers = null;

        [XmlIgnore]
        private Animator m_Animator;

        private string m_Output; 
        private int? m_NameHash = null; 

        public override void ResetState()
        {
            this.m_Animator = null;
            this.m_Output = null;
        }

        public override string Update(XMLScriptInstance cutscene, XMLScriptController controller)
        {
            if (this.m_Animator == null)
            {
                Transform t = string.IsNullOrEmpty(this.Target)
                    ? controller.transform
                    : controller.FindProxy(this.Target);

                if (t==null)
                {
                    Debug.LogWarning($"Can't find target - TARGET:'{Target}'");
                    return OUTPUT_NEXT;
                }

                this.m_Animator = t.GetComponent<Animator>();

                if (this.m_Animator==null)
                {
                    Debug.LogWarning($"Can't find animator on target - TARGET:'{Target}'");
                    return OUTPUT_NEXT;
                }

                if (Trigger != null)
                    this.m_Animator.SetTrigger(Trigger);

                if (Idle != BehaviourAlias.NONE)
                    this.m_Animator.SetInteger("idle", (int) Idle);
                    
                if (Face != BehaviourAlias.NONE)
                    this.m_Animator.SetInteger("face", (int) Face);

                if (this.Booleans != null)
                    foreach (var i in this.Booleans)
                        this.m_Animator.SetBool(i.Name, i.Value);

                if (this.Integers != null)
                    foreach (var i in this.Integers)
                        this.m_Animator.SetInteger(i.Name, i.Value);

                if (this.waitEndOfClip < 0)
                    this.m_Output = OUTPUT_NEXT;
            }
            else if (waitEndOfClip >= 0)
            {
                var ci = this.m_Animator.GetCurrentAnimatorStateInfo(waitEndOfClip);

                if (this.m_NameHash == null)
                    this.m_NameHash = ci.fullPathHash;
                else if (this.m_NameHash != ci.fullPathHash)
                    this.m_Output = OUTPUT_NEXT;
                
                if (ci.normalizedTime>=1)
                    this.m_Output = OUTPUT_NEXT;
            }

            return this.m_Output;
        }

        public enum BehaviourAlias
        {
            NONE = 0,
            CUTE = 1,
            DISAPPROVE = 2,
            SERIOUS = 3,
        }

        public class AnimBool
        {
            [XmlAttribute("name")] public string Name = null;
            [XmlAttribute("value")] public bool Value = false;
        }

        public class AnimInt
        {
            [XmlAttribute("name")] public string Name = null;
            [XmlAttribute("value")] public int Value = 0;
        }

    }

    
}