using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class OutputAction : XMLScriptAction
    {
        [XmlAttribute("node")]
        public string Node = null;

        public override string Update(XMLScriptInstance xmlscript, XMLScriptController controller)
        {
            return ResourceString.Parse(Node, controller.Game.GetVariable);
        }

    }
}