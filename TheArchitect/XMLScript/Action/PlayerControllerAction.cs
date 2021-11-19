using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Model;
using TheArchitect.SceneObjects;
using TheArchitect.Core;

namespace TheArchitect.XMLScript.Action
{
    public class PlayerControllerAction : XMLScriptAction
    {
        [XmlAttribute("target")]
        public string Target = null;

        public override string Update(XMLScriptInstance cutscene, XMLScriptController controller)
        {

            return null;
        }

    }
}