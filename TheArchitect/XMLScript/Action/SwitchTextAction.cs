using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{

    public class SwitchTextAction : XMLScriptAction
    {
        [XmlAttribute("flag")]
        public string Flag;
        [XmlAttribute("string")]
        public string String;
        [XmlAttribute("output")]
        public string OutputStr;
        [XmlElement("case")]
        public SwitchCase[] Cases;
        
        public override string Update(XMLScriptInstance xmlscript, XMLScriptController controller)
        {
            string value = "";
            if (!string.IsNullOrEmpty(this.Flag))
                value = controller.Game.GetVariable(this.Flag, 0).ToString();

            if (!string.IsNullOrEmpty(this.String))
                value = controller.Game.GetVariable(this.String, "").ToString();

            foreach (var c in Cases)
            {
                string eq = ResourceString.Parse(c.Eq, controller.Game.GetVariable);
                if (eq == value || c.Eq == "#default" )
                {
                    controller.Game.SetVariable(OutputStr, ResourceString.Parse(c.Then, controller.Game.GetVariable));
                    return OUTPUT_NEXT;
                }
            }

            controller.Game.SetVariable(OutputStr, null);
            return OUTPUT_NEXT;
        }

    }

    public class SwitchCase 
    {
        [XmlAttribute("eq")]
        public string Eq;
        [XmlText]
        public string Then;
    }
}