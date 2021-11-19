using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{

    public class StringAction : XMLScriptAction
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("ref")]
        public string Ref;
        [XmlAttribute("copy")]
        public string Copy = null;
        [XmlAttribute("set")]
        public string Set = null;
        [XmlAttribute("unset")]
        public bool Unset = false;
        [XmlText]
        public string Message = null;
        
        public override string Update(XMLScriptInstance cutscene, XMLScriptController controller)
        {
            string resolvedName = Name;
            if (!string.IsNullOrEmpty(Ref))
            {
                resolvedName = controller.Game.GetVariable(Ref, Name);
            }

            if (!string.IsNullOrEmpty(Copy))
            {
                string v = controller.Game.GetVariable(Copy, "");
                controller.Game.SetVariable(resolvedName, v);
            }

            if (!string.IsNullOrEmpty(Set ))
            {
                controller.Game.SetVariable(resolvedName, ResourceString.Parse(Set, controller.Game.GetVariable));
            }

            if (Unset)
            {
                controller.Game.UnsetVariable(resolvedName);
            }

            if (!string.IsNullOrEmpty(Message))
            {
                LogAction log = new LogAction() { Text = Message };
                controller.StartCoroutine(log.Log(controller));
            }

            return OUTPUT_NEXT;
        }

    }
}