using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.MonoBehaviour;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public struct DialogMessage
    {
        [XmlAttribute("wait")]
        public float Wait;
        [XmlAttribute("instant")]
        public bool Instant;
        [XmlText]
        public string Text;
    }

    public class DialogAction : XMLScriptAction
    {
        [XmlElement("m")] public DialogMessage[] Messages = null;
        [XmlAttribute("char")] public string CharacterId = null;
        [XmlAttribute("track")] public string Track = null;
        [XmlAttribute("style")] public DialogStyle Style = DialogStyle.SUBJECTIVE;
        [XmlAttribute("ttl")] public float TimeToLive;
        
        [XmlIgnore] private PanelDialog m_Panel;
        [XmlIgnore] private CharacterData m_Character;
        [XmlIgnore] private int m_CurrentMessage = -1;
        [XmlIgnore] private int m_NextMessage = 0;
        [XmlIgnore] private float m_EndWaitTime = 0;
        [XmlIgnore] private float m_StartTime = 0;

        [XmlIgnore] private Coroutine m_LoadCoroutine;

        private IEnumerator Load(XMLScriptController controller)
        {
            this.m_StartTime = Time.time;

            if (!string.IsNullOrEmpty(this.CharacterId))
            {
                var handleCharacter = Addressables.LoadAssetAsync<Character>($"characters/{this.CharacterId}.asset");
            
                yield return handleCharacter;

                if (handleCharacter.Status==AsyncOperationStatus.Succeeded)
                {
                    this.m_Character = handleCharacter.Result.Data;
                    this.m_Character.DefaultDisplayName = controller.Game.GetVariable<string>($"CHAR:{this.CharacterId}:NAME", this.m_Character.DefaultDisplayName);
                    Addressables.Release(handleCharacter);
                }
            }

            var handleDialog = Addressables.InstantiateAsync("panel-dialog.prefab", controller.transform);
            yield return handleDialog;

            this.m_Panel = handleDialog.Result.GetComponent<PanelDialog>();
            
            this.m_Panel.TrackedTransform = Track == null
                ? controller.FindProxy(CharacterId)
                : controller.FindProxy(Track);
            
            if (this.m_Panel.TrackedTransform!=null)
            {
                var dtt = this.m_Panel.TrackedTransform.GetComponent<DialogTrackTarget>();
                if (dtt != null)
                    this.m_Panel.TrackedTransform = dtt.DialogTarget;
            }

            if (this.m_Panel.TrackedTransform != null)
                this.Style = DialogStyle.OBJECTIVE;

            this.m_Panel.gameObject.SetActive(false);
        }

        public override string Update(XMLScriptInstance instance, XMLScriptController controller)
        {
            if (this.m_LoadCoroutine == null)
                this.m_LoadCoroutine = controller.StartCoroutine(Load(controller));
            
            if (this.m_Panel == null)
                return null;

            #if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.X))
                this.m_EndWaitTime = 0;
            #endif

            if (this.m_EndWaitTime > 0 && this.m_EndWaitTime > Time.time)
                return null;

            if (this.TimeToLive > 0 && Time.time > this.m_StartTime + this.TimeToLive)
            {
                ResetState();
                return OUTPUT_NEXT;
            }

            if (this.m_NextMessage != this.m_CurrentMessage)
            {
                this.m_CurrentMessage = this.m_NextMessage;
                this.m_Panel.gameObject.SetActive(true);
                this.m_Panel.Display(
                    this.m_Character,
                    ResourceString.Parse(this.Messages[this.m_CurrentMessage].Text, controller.Game.GetVariable),
                    this.Messages[this.m_CurrentMessage].Instant,
                    this.Style
                );
            } else if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Jump"))
            {
                if (this.m_Panel.HasRunningDisplay())
                {
                    this.m_Panel.SkipDisplay();
                }
                else if (this.Messages[this.m_CurrentMessage].Wait > 0 && this.Messages.Length > this.m_CurrentMessage + 1)
                {
                    this.m_Panel.gameObject.SetActive(false);
                    this.m_EndWaitTime = Time.time + this.Messages[this.m_CurrentMessage].Wait;
                    this.m_NextMessage++;
                }
                else if (this.Messages.Length > this.m_CurrentMessage + 1)
                {
                    this.m_NextMessage++;
                }
                else
                {
                    ResetState();
                    return OUTPUT_NEXT;
                }
            }
            
            return null;
        }

        public override void ResetState()
        {
            this.m_LoadCoroutine = null;
            this.m_CurrentMessage = -1;
            this.m_NextMessage = 0;
            this.m_EndWaitTime = 0;
            this.m_StartTime = 0;
            this.m_Character = new CharacterData();

            if (this.m_Panel != null)
                Addressables.ReleaseInstance(this.m_Panel.gameObject);

            this.m_Panel = null;
        }


    }
}