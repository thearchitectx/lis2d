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
        SAVE = 3,
        QUEST = 4,
    }

    public class LogAction : XMLScriptAction
    {
        [XmlAttribute("icon")]
        public UIIcon Icon = UIIcon.NONE;
        [XmlText]
        public string Text;

        private Coroutine m_Coroutine = null;
        private string m_Output = null;
        
        public override void ResetState()
        {
            this.m_Coroutine = null;
            this.m_Output = null;
        }

        public override string Update(XMLScriptInstance instance, XMLScriptController controller)
        {
            if (this.m_Coroutine == null)
                this.m_Coroutine = controller.StartCoroutine(
                    Log(controller)
                );
            
            return this.m_Output;
        }

        private static bool m_HasInstanceLoadingLogger = false;
        
        public System.Collections.IEnumerator Log(XMLScriptController controller)
        {
            while (LogAction.m_HasInstanceLoadingLogger)
                yield return new WaitWhile( () => LogAction.m_HasInstanceLoadingLogger );

            try
            {
                LogAction.m_HasInstanceLoadingLogger = true;

                PanelLogger logger = GameObject.FindWithTag("Logger")?.GetComponentInChildren<PanelLogger>();
                if (logger == null)
                {
                    var handlePanel = Addressables.InstantiateAsync("panel-logger.prefab");

                    handlePanel.Completed += (h) => {
                        logger = handlePanel.Result.GetComponent<PanelLogger>();
                    };

                    yield return handlePanel;
                }

                Sprite icon = null;
                if (this.Icon != UIIcon.NONE)
                {
                    var handleIcon = Addressables.LoadAssetAsync<Sprite>($"icon-{this.Icon.ToString().ToLower()}");
                    yield return handleIcon;

                    icon = handleIcon.Result;
                }

                logger.AddItem(ResourceString.Parse(this.Text, controller.Game.GetVariable), icon);
            }
            finally
            {
                LogAction.m_HasInstanceLoadingLogger = false;
                this.m_Output = OUTPUT_NEXT;
            }

        }

    }
}