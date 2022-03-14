using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class SetProxy : XMLScriptAction
    {
        [XmlAttribute("target")]
        public string Target = null;
        [XmlAttribute("name")]
        public string Name = null;

        [XmlAttribute("clear")]
        public bool Clear = false;

        public override string Update(XMLScriptInstance instance, XMLScriptController controller)
        {
            if (Clear)
             controller.ClearProxies();
             
            Transform t = controller.FindProxy(Target);
            controller.AddProxy(Name, t);
            return OUTPUT_NEXT;
        }

    }
}