using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class DebugLogAction : XMLScriptAction
    {
        [XmlText]
        public string Message;

        public override string Update(XMLScriptInstance cutscene, XMLScriptController controller)
        {
            Debug.Log(ResourceString.Parse(this.Message, controller.Game.GetVariable));
            return OUTPUT_NEXT;
        }
    }

}
