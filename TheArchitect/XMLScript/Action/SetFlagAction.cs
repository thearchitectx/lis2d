using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{

    public class SetFlagAction : XMLScriptAction
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("ref")]
        public string Ref;
        [XmlAttribute("copy")]
        public string Copy = null;
        [XmlAttribute("inc")]
        public string Inc = null;
        [XmlAttribute("set")]
        public string Set = null;
        [XmlAttribute("bit-on")]
        public string BitOn = null;
        [XmlAttribute("bit-off")]
        public string BitOff = null;
        [XmlAttribute("icon")]
        public UIIcon MessageIcon = UIIcon.NONE;
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
                int v = controller.Game.GetVariable(Copy, 0);
                controller.Game.SetVariable(resolvedName, v);
            }

            if (!string.IsNullOrEmpty(Inc))
            {
                int v = controller.Game.GetVariable(resolvedName, 0);
                controller.Game.SetVariable(resolvedName, v + ResourceString.ParseToInt(Inc, controller.Game.GetVariable));
            }

            if (!string.IsNullOrEmpty(Set ))
            {
                controller.Game.SetVariable(resolvedName, ResourceString.ParseToInt(Set, controller.Game.GetVariable));
            }

            if (!string.IsNullOrEmpty(BitOff ))
            {
                int v = controller.Game.GetVariable(resolvedName, 0);
                controller.Game.SetVariable(resolvedName, v & ~(1 << ResourceString.ParseToInt(BitOff, controller.Game.GetVariable)));
            }

            if (!string.IsNullOrEmpty(BitOn ))
            {
                int v = controller.Game.GetVariable(resolvedName, 0);
                controller.Game.SetVariable(resolvedName, v | 1 << ResourceString.ParseToInt(BitOn, controller.Game.GetVariable));
            }

            if (!string.IsNullOrEmpty(Message))
            {
                LogAction log = new LogAction() { Text = Message, Icon = this.MessageIcon };
                controller.StartCoroutine(log.Log(controller));
            }

            return OUTPUT_NEXT;
        }

    }
}