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
        public string Log = "Autosaving...";

        public override string Update(XMLScriptInstance xmlscript, XMLScriptController controller)
        {
            LogAction logAction = new LogAction() { Text = Log, Icon = UIIcon.SAVE };
            controller.StartCoroutine(logAction.Log(controller));

            string rootPath = Application.persistentDataPath;
            string label = string.IsNullOrEmpty(Label) ? "AUTOSAVE" : $"AUTOSAVE ({ResourceString.Parse(this.Label, controller.Game.GetVariable)})";
            string slot = "autosave";
            string version = Application.version;
            
            controller.Game.SetVariable(GameState.SYSTEM_SCRIPT_PATH_VARIABLE, controller.ScriptPath );
            controller.Game.SetVariable(GameState.SYSTEM_SCRIPT_NODE_VARIABLE, xmlscript.CurrentNode.Id );

            controller.StartCoroutine( GameStateIO.SaveScreenshot(rootPath, slot) );
            
            var state = controller.Game.BuildStateInstance();
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.DoWork += (sender, args) => {
                args.Result = GameStateIO.Save(
                    state,
                    rootPath,
                    slot,
                    ResourceString.Parse(label, controller.Game.GetVariable),
                    version
                );
            };
            worker.RunWorkerCompleted += (sender, args) => {
                Debug.Log($"Autosaved at {args.Result}");
            };
            worker.RunWorkerAsync();

            return OUTPUT_NEXT;
        }
    }
}