using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.Game;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class BootstrapScriptAction : XMLScriptAction
    {
        [XmlAttribute("script")]
        public string ScriptPath = null;
        [XmlAttribute("spawn")]
        public string Spawn = null;
        [XmlAttribute("node")]
        public string Node = null;

        public override string Update(XMLScriptInstance instance, XMLScriptController controller)
        {
            if (ScriptPath=="title")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
                return null;
            }

            var player = controller.FindProxy("#Player");
            if (player != null)
            {
                controller.Game.SetVariable(GameState.SYSTEM_PLAYER_X, player.localPosition.x);
                controller.Game.SetVariable(GameState.SYSTEM_PLAYER_Y, player.localPosition.y);
            }

            if (!string.IsNullOrEmpty(Spawn))
            {
                controller.Game.SetVariable(GameState.SYSTEM_PLAYER_SPAWN, ResourceString.Parse(Spawn, controller.Game.GetVariable) );
            }
            else
            {
                controller.Game.UnsetVariable(GameState.SYSTEM_PLAYER_SPAWN);
            }

            if (string.IsNullOrEmpty(ScriptPath))
            {
                this.ScriptPath = controller.ScriptPath;
            }

            controller.Game.SetVariable(GameState.SYSTEM_SCRIPT_PATH_VARIABLE, ResourceString.Parse(ScriptPath, controller.Game.GetVariable));

            if (string.IsNullOrEmpty(Node))
                controller.Game.UnsetVariable(GameState.SYSTEM_SCRIPT_NODE_VARIABLE);
            else
                controller.Game.SetVariable(GameState.SYSTEM_SCRIPT_NODE_VARIABLE, Node);

            UnityEngine.SceneManagement.SceneManager.LoadScene("XMLScriptScene");

            return null;
        }

    }
}