using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.Game;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class BootstrapScriptAction : XMLScriptAction
    {
        [XmlAttribute("script")]
        public string ScriptPath = null;
        [XmlAttribute("node")]
        public string Node = null;

        public override string Update(XMLScriptInstance instance, XMLScriptController controller)
        {
            var player = controller.FindProxy("#Player");
            if (player != null)
            {
                controller.Game.SetVariable(GameState.SYSTEM_PLAYER_X, player.localPosition.x);
                controller.Game.SetVariable(GameState.SYSTEM_PLAYER_Y, player.localPosition.y);
            }

            controller.Game.SetVariable(GameState.SYSTEM_SCRIPT_PATH_VARIABLE, ScriptPath);

            if (string.IsNullOrEmpty(Node))
                controller.Game.UnsetVariable(GameState.SYSTEM_SCRIPT_NODE_VARIABLE);
            else
                controller.Game.SetVariable(GameState.SYSTEM_SCRIPT_NODE_VARIABLE, Node);

            UnityEngine.SceneManagement.SceneManager.LoadScene("XMLScriptScene");

            return null;
        }

    }
}