using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Model;
using TheArchitect.SceneObjects;
using TheArchitect.Core;

namespace TheArchitect.XMLScript.Action
{
    public class ObjectAction : XMLScriptAction
    {
        [XmlAttribute("target")]
        public string Target = "";
        [XmlAttribute("target-key")]
        public string TargetKey = "";
        [XmlAttribute("active")]
        public bool Active = true;
        [XmlAttribute("destroy")]
        public int Destroy = -1;
        [XmlElement("message")]
        public ObjectMessage[] Messages = null;
        [XmlElement("outcome")]
        public ObjectOutcome[] Outcomes = null;

        private SceneObject m_SceneObject;
        private Transform m_Target;

        public override void ResetState()
        {
            this.m_SceneObject = null;
            this.m_Target = null;
        }

        public override string Update(XMLScriptInstance cutscene, XMLScriptController controller)
        {
            if (this.m_Target == null)
            {
                Target = !string.IsNullOrEmpty(Target) ? Target : TargetKey;
                
                this.m_Target = controller.FindProxy(Target);
                if (this.m_Target == null)
                {
                    Debug.LogWarning($"Can't find resolve object '{Target}'");
                    return OUTPUT_NEXT;
                }
            
                this.m_SceneObject = this.m_Target.GetComponent<SceneObject>();

                this.m_Target.gameObject.SetActive(this.Active);

                SendMessages(Messages, this.m_Target.gameObject, controller.Game.GetVariable);

                if (Destroy == 0)
                {
                    if (!Addressables.ReleaseInstance(this.m_Target.gameObject))
                        GameObject.Destroy(this.m_Target.gameObject);
                    
                    return OUTPUT_NEXT;
                }
                else if (Destroy > 0)
                    controller.StartCoroutine(DestroyAfter(this.m_Target.gameObject, Destroy));
            }

            if (this.m_SceneObject != null && this.Outcomes != null && this.Outcomes.Length > 0)
            {
                foreach (var outcome in this.Outcomes)
                {
                    if ((!string.IsNullOrEmpty(this.m_SceneObject.Outcome) && outcome.Value == "*") || outcome.Value == this.m_SceneObject.Outcome)
                    {
                        if (!string.IsNullOrEmpty(outcome.CopyTo))
                            controller.Game.SetVariable(outcome.CopyTo, this.m_SceneObject.Outcome);

                        if (outcome.Destroy)
                            if (!Addressables.ReleaseInstance(this.m_SceneObject.gameObject))
                                GameObject.Destroy(this.m_SceneObject.gameObject);

                        if (outcome.Clear)
                            this.m_SceneObject.ClearOutcome();
                            
                        if (!string.IsNullOrEmpty(outcome.Node))
                            return outcome.Node;

                        return OUTPUT_NEXT;
                    }
                }

                return null;
            }

            return OUTPUT_NEXT;
            

        }

        private static System.Collections.IEnumerator DestroyAfter(GameObject go, float time)
        {
            yield return new WaitForSeconds(time);

            if (!Addressables.ReleaseInstance(go))
                        GameObject.Destroy(go);
        }

        public static void SendMessages(ObjectMessage[] messages, GameObject target, System.Func<string, object> context)
        {
            if (messages != null)
                foreach (var message in messages)
                {
                    try
                    {
                        if (message.Param != null)
                        {
                            var value = message.Param.Value;
                            if (message.Param.Value is string)
                                value = ResourceString.Parse((string) value, context);

                            if (message.Broadcast)
                                target.BroadcastMessage(message.Name, value, SendMessageOptions.RequireReceiver);
                            else
                                target.SendMessage(message.Name, value, SendMessageOptions.RequireReceiver);
                        }
                        else
                        {
                            if (message.Broadcast)
                                target.BroadcastMessage(message.Name, SendMessageOptions.RequireReceiver);
                            else
                                target.SendMessage(message.Name, SendMessageOptions.RequireReceiver);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
        }

    }

    public class ObjectMessage
    {
        [XmlAttribute("name")]
        public string Name = null;
        [XmlAttribute("broadcast")]
        public bool Broadcast = false;
        [
            XmlElement("string", typeof(StringParam)),
            XmlElement("int", typeof(IntParam)),
            XmlElement("float", typeof(FloatParam)),
            XmlElement("bool", typeof(BoolParam))
        ]
        public Param Param;
        
    }

    public class ObjectOutcome
    {
        [XmlAttribute("value")]
        public string Value = "*";
        [XmlAttribute("node")]
        public string Node = null;
        [XmlAttribute("copy-to")]
        public string CopyTo = null;
        [XmlAttribute("clear")]
        public bool Clear = false;
        [XmlAttribute("destroy")]
        public bool Destroy = false;
    }

    public class Param { public virtual object Value { get { return null; } } }
    public class StringParam : Param { [XmlAttribute("value")] public string m_Value = null; public override object Value { get { return this.m_Value; } } }
    public class IntParam : Param { [XmlAttribute("value")] public int m_Value = 0; public override object Value { get { return this.m_Value; } } }
    public class FloatParam : Param { [XmlAttribute("value")] public float m_Value = 0; public override object Value { get { return this.m_Value; } } }
    public class BoolParam : Param { [XmlAttribute("value")] public bool m_Value = false; public override object Value { get { return this.m_Value; } } }
}