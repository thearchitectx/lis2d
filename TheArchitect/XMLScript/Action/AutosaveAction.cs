using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Model;
using TheArchitect.Game;
using TheArchitect.Core;

namespace TheArchitect.XMLScript.Action
{

    public class AutosaveAction : XMLScriptAction
    {
        [XmlAttribute("label")]
        public string Label;
        [XmlAttribute("log")]
        public string Log;

        public override string Update(XMLScriptInstance xmlscript, XMLScriptController controller)
        {
            string rootPath = Application.persistentDataPath;
            string label = string.IsNullOrEmpty(Label) ? "Save" : ResourceString.Parse(this.Label, controller.Game.GetVariable);
            string version = Application.version;

            LogAction logAction = new LogAction() { Text = string.IsNullOrEmpty(Log)?$"Autosaving..." :Log, Icon = UIIcon.SAVE };
            logAction.Log(controller);

            controller.Game.SetVariable(GameState.SYSTEM_SCRIPT_PATH_VARIABLE, controller.ScriptPath );
            controller.Game.SetVariable(GameState.SYSTEM_SCRIPT_NODE_VARIABLE, xmlscript.CurrentNode.Id );

            controller.StartCoroutine( GameStateIO.SaveScreenshot(rootPath, GameStateIO.AUTOSAVE_SLOT) );
            
            var state = controller.Game.BuildStateInstance();
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.DoWork += (sender, args) => {
                args.Result = GameStateIO.Save(
                    state,
                    rootPath,
                    GameStateIO.AUTOSAVE_SLOT,
                    ResourceString.Parse(label, controller.Game.GetVariable),
                    version
                );
            };
            worker.RunWorkerCompleted += (sender, args) => {
                Debug.Log($"Saved at {args.Result}");
            };
            worker.RunWorkerAsync();

            return OUTPUT_NEXT;
        }
    }
}