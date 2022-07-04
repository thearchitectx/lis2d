using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Model;
using TheArchitect.Core;
using TheArchitect.Game;

namespace TheArchitect.XMLScript.Action
{
    public class AnimateAction : XMLScriptAction
    {
        [XmlAttribute("trigger")]
        public string Trigger = null;
        [XmlAttribute("target")]
        public string Target = null;
        [XmlAttribute("inc")]
        public string IntInc = null;
        [XmlAttribute("idle")]
        public IdleAlias Idle = IdleAlias.NONE;
        [XmlAttribute("face")]
        public FaceAlias Face = FaceAlias.NONE;
        [XmlAttribute("look")]
        public LookAlias Look = LookAlias.NONE;
        [XmlAttribute("waitEndOfClip")]
        public int waitEndOfClip = -1;
        [XmlAttribute("speed")]
        public float speed = float.NaN;
        [XmlElement("bool")]
        public AnimBool[] Booleans = null;
        [XmlElement("int")]
        public AnimInt[] Integers = null;
        [XmlElement("inc")]
        public AnimIntInc[] IntIncs = null;

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

                if (!float.IsNaN(this.speed))
                    this.m_Animator.speed = this.speed;

                if (Trigger != null)
                    this.m_Animator.SetTrigger(ResourceString.Parse(Trigger, controller.Game.GetVariable));

                if (IntInc != null)
                    this.m_Animator.SetInteger(IntInc, this.m_Animator.GetInteger(IntInc) + 1);

                if (Idle != IdleAlias.NONE)
                    this.m_Animator.SetInteger("idle", (int) Idle);
                    
                if (Face != FaceAlias.NONE)
                    this.m_Animator.SetInteger("face", (int) Face);
                    
                if (Look != LookAlias.NONE)
                    this.m_Animator.SetInteger("look", (int) Look);

                if (this.Booleans != null)
                    foreach (var i in this.Booleans)
                        this.m_Animator.SetBool(i.Name, i.Value);

                if (this.Integers != null)
                    foreach (var i in this.Integers)
                        this.m_Animator.SetInteger(i.Name, i.Value);

                if (this.IntIncs != null)
                    foreach (var i in this.IntIncs)
                        this.m_Animator.SetInteger(i.Name, this.m_Animator.GetInteger(i.Name) + i.Value);

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

        public enum LookAlias
        {
            NONE = -100,
            LEFT = -1,
            NEUTRAL = 0,
            RIGHT = 1
        }

        public enum FaceAlias
        {
            NONE = 0,
            CUTE = 1,
            DISAPPROVE = 2,
            NEUTRAL = 3,
            UPSET = 4,
            SIGH = 5,
            SURPRISED = 6,
            EMBARRASSED = 7
        }

        public enum IdleAlias
        {
            NONE = 0,
            CUTE = 1,
            NEUTRAL = 3,
            UPSET = 4
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

        public class AnimIntInc
        {
            [XmlAttribute("name")] public string Name = null;
            [XmlAttribute("value")] public int Value = 1;
        }

    }

    
}