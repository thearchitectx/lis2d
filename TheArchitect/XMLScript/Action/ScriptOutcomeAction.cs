using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class ScriptOutcomeAction : XMLScriptAction
    {
        [XmlAttribute("value")]
        public string Value = null;

        public override string Update(XMLScriptInstance instance, XMLScriptController controller)
        {
            instance.Outcome = Value;
            return OUTPUT_END;
        }

    }
}