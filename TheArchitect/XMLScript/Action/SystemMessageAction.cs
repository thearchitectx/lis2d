using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class SystemMessageAction : XMLScriptAction
    {
        [XmlText]
        public string Message = "Message";
        [XmlAttribute("simple")]
        public bool Simple = false;
        
        private Coroutine m_LoadCoroutine;
        private GameObject m_Panel;

        public override void ResetState()
        {
            this.m_LoadCoroutine = null;
            this.m_Panel = null;
        }

        public override string Update(XMLScriptInstance cutscene, XMLScriptController controller)
        {
            
            if (this.m_LoadCoroutine == null)
            {
                this.m_LoadCoroutine = controller.StartCoroutine(Load(controller));
            }

            if (this.m_Panel != null && (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Jump")))
            {
                controller.StartCoroutine(Release());
                return OUTPUT_NEXT;
            }
            
            return null;
        }

        private IEnumerator Release()
        {
            this.m_Panel.GetComponentInChildren<Animator>().SetBool("shown", false);
            yield return new WaitForSeconds(0.5f);
            Addressables.ReleaseInstance(this.m_Panel);
        }

        private IEnumerator Load(XMLScriptController controller)
        {
            var handle = Addressables.InstantiateAsync(
                Simple ? "panel-simple-message.prefab" : "panel-system-message.prefab", 
                controller.transform, false, true);

            yield return handle;

            this.m_Panel = handle.Result;

            UnityEngine.UI.Text text = this.m_Panel.GetComponentInChildren<UnityEngine.UI.Text>();
            if (text != null)
            {
                text.text = ResourceString.Parse(Message, controller.Game.GetVariable) ;
            }
        }

    }
}