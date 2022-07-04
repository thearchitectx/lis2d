using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Xml.Serialization;
using TheArchitect.MonoBehaviour;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{

    public enum UIIcon
    {
        NONE = 0,
        PARAGON = 1,
        RENEGADE = 2,
        RIFT = 8,
        SAVE = 3,
        QUEST = 4,
        KEY = 5,
        LOCKPICK = 6,
        MONEY = 7,
        BUTTERFLY = 9,
        AFFINITY = 10,
        JOINT = 11,
        CORRUPTION = 12,
        BAND = 13,
        CALENDAR = 14,
        MIND = 15
    }

    public class LogAction : XMLScriptAction
    {
        [XmlAttribute("icon")]
        public UIIcon Icon = UIIcon.NONE;
        [XmlText]
        public string Text;

        public override string Update(XMLScriptInstance instance, XMLScriptController controller)
        {
            Log(controller);
            return OUTPUT_NEXT;
        }

        public void Log(XMLScriptController controller)
        {
            try
            {
                PanelLogger logger = GameObject.FindWithTag("Logger")?.GetComponentInChildren<PanelLogger>();

                string icon = null;
                if (this.Icon != UIIcon.NONE)
                {
                    icon = $"icon-{this.Icon.ToString().ToLower()}";
                }

                logger.AddItem(ResourceString.Parse(this.Text, controller.Game.GetVariable), icon);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }

        }

    }
}