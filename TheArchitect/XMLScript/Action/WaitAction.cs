using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class WaitAction : XMLScriptAction
    {
        [XmlAttribute("time")]
        public float WaitTime = 1;
        [XmlAttribute("rnd")]
        public float Rnd = 0;

        private float m_EndTime = 0;

        public override void ResetState()
        {
            this.m_EndTime = 0;
        }

        public override string Update(XMLScriptInstance instance, XMLScriptController controller)
        {
            if (this.m_EndTime == 0)
                this.m_EndTime = Rnd > 0
                    ? Time.time + UnityEngine.Random.Range( WaitTime - Rnd, WaitTime + Rnd)
                    : Time.time + WaitTime;

            #if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.X)) {
                return OUTPUT_NEXT;
            }
            #endif

            return UnityEngine.Time.time >= m_EndTime ? OUTPUT_NEXT : null;
        }

    }
}