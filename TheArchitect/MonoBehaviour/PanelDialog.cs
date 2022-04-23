using System.Xml.Serialization;
using System.Collections;
using TheArchitect.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace TheArchitect.MonoBehaviour
{
    public enum DialogStyle
    {
        [XmlEnum(Name="OBJECTIVE")]
        OBJECTIVE,
        [XmlEnum(Name="SUBJECTIVE")]
        SUBJECTIVE
    }

    public class PanelDialog : UnityEngine.MonoBehaviour
    {
        const int PADDING_MESSAGE_LEFT = 15;
        const int PADDING_MESSAGE_RIGHT = 15;
        const int PADDING_MESSAGE_BOTTOM = 5;
        const int PADDING_MESSAGE_TOP = 0;
        const int PANEL_WIDTH = 0;

        [SerializeField] public Transform TrackedTransform = null;
        [SerializeField] private Sprite m_SpriteLeft;
        [SerializeField] private Sprite m_SpriteRight;
        [SerializeField] private Sprite m_SpriteSubjective;
        [SerializeField] private Sprite m_SpriteThought;
        [SerializeField] public RectTransform m_MainRectTransform;
        [SerializeField] public bool ClampToScreen = true;
        [SerializeField] public Image m_ImageForward = null;
        [SerializeField] public Image m_ImageDialogBack = null;
        [SerializeField] public Image m_ImageCharacterName = null;
        [SerializeField] public Text m_TextMessage = null;
        [SerializeField] public Text m_TextName = null;
        [SerializeField] public AudioSource m_BlipAudioSource = null;

        private string m_Message = null;
        private int m_MessagePosition = 0;
        private float m_WriteTimeInterval = 0.025f;
        private RectTransform m_CanvasTransform;
        private bool m_IsRollingText;
        private DialogStyle m_Style;
        private bool m_LastFrameWasRight;
        private bool m_IsThought;

        public bool IsRollingText { get { return m_IsRollingText; } }
        public bool HasPendingText { get { return this.m_MessagePosition < this.m_Message.Length; } }

        public void Display(CharacterData character, string message, bool thought = false, DialogStyle style = DialogStyle.SUBJECTIVE)
        {
            this.m_Style = style;
            this.m_IsThought = thought;

            this.m_ImageCharacterName.gameObject.SetActive(!string.IsNullOrEmpty(character.DefaultDisplayName));
            this.m_TextName.gameObject.SetActive(!string.IsNullOrEmpty(character.DefaultDisplayName));
            if (!string.IsNullOrEmpty(character.DefaultDisplayName))
            {
                this.m_ImageCharacterName.color = character.Color;
                this.m_TextName.color = character.ColorContrast;
                this.m_TextName.text = character.DefaultDisplayName;
            }

            StopAllCoroutines();
            StartCoroutine(_DisplayTextMessage(character, message, thought));
        }

        public void SkipDisplay()
        {
            this.m_MessagePosition = this.m_Message.Length - 1;
        }

        public bool HasRunningDisplay()
        {
            return this.m_MessagePosition < this.m_Message.Length;
        }

        void Start()
        {
            var canvas = GetComponent<Canvas>();
            this.m_CanvasTransform = canvas.transform.GetComponent<RectTransform>();
            this.m_ImageForward.gameObject.SetActive(false);
            this.m_ImageDialogBack.sprite = null;
            this.m_MainRectTransform.anchoredPosition = new Vector2(-10000, -100000);
            this.m_TextMessage.text = "";
        }

        void Update()
        {
            if (Time.deltaTime==0)
            {
                m_BlipAudioSource.enabled = false;
                return;
            }

            if (this.TrackedTransform != null && this.m_Style != DialogStyle.SUBJECTIVE)
            {
                Vector3 pos = Camera.main.WorldToViewportPoint(TrackedTransform.position);
                pos = new Vector2(
                    pos.z >= 0 ? pos.x * this.m_CanvasTransform.rect.width : 0,
                    (1 - pos.y) * this.m_CanvasTransform.rect.height
                );

                bool right = pos.x + ( this.m_LastFrameWasRight ? 25 : -25 ) > this.m_CanvasTransform.rect.width / 2;

                this.m_LastFrameWasRight = right;

                pos = new Vector2(
                    right ? pos.x - this.m_MainRectTransform.rect.width : pos.x,
                    - pos.y
                );

                if (this.ClampToScreen)
                {
                    pos.y = Mathf.Clamp(pos.y, -this.m_CanvasTransform.rect.height+m_ImageDialogBack.rectTransform.rect.height+20 , -20 );
                    pos.x = Mathf.Clamp(pos.x, 0, this.m_CanvasTransform.rect.width - m_ImageDialogBack.rectTransform.rect.width);
                }

                this.m_MainRectTransform.anchoredPosition = pos;
                this.m_ImageForward.rectTransform.anchoredPosition = new Vector2(
                    right ? 325 : 350,
                    right ? 10 : 0
                );
                
                var spr = right ? m_SpriteRight : m_SpriteLeft;
                if (spr != this.m_ImageDialogBack.sprite)
                {
                    this.m_ImageDialogBack.sprite = spr;
                    m_ImageDialogBack.GetComponent<LayoutGroup>().padding = new RectOffset(
                        (int) this.m_ImageDialogBack.sprite.border.x + PADDING_MESSAGE_LEFT,
                        (int) this.m_ImageDialogBack.sprite.border.z + PADDING_MESSAGE_RIGHT,
                        (int) this.m_ImageDialogBack.sprite.border.w + PADDING_MESSAGE_TOP,
                        (int) this.m_ImageDialogBack.sprite.border.y + PADDING_MESSAGE_BOTTOM
                    );
                }
            }
            else
            {
                var spr = (this.m_Style == DialogStyle.SUBJECTIVE)  ? this.m_SpriteSubjective : this.m_SpriteLeft;
                if (this.m_IsThought)
                    spr = this.m_SpriteThought;
                    
                if (spr != this.m_ImageDialogBack.sprite)
                {
                    this.m_ImageDialogBack.sprite = spr;
                    m_ImageDialogBack.GetComponent<LayoutGroup>().padding = new RectOffset(
                        (int) this.m_ImageDialogBack.sprite.border.x + PADDING_MESSAGE_LEFT,
                        (int) this.m_ImageDialogBack.sprite.border.z + PADDING_MESSAGE_RIGHT,
                        (int) this.m_ImageDialogBack.sprite.border.w + PADDING_MESSAGE_TOP,
                        (int) this.m_ImageDialogBack.sprite.border.y + PADDING_MESSAGE_BOTTOM
                    );
                }
                this.m_ImageForward.rectTransform.anchoredPosition = new Vector2(355, 0);
                this.m_MainRectTransform.anchoredPosition = new Vector2(
                    (this.m_CanvasTransform.rect.width / 2) - ( m_ImageDialogBack.rectTransform.rect.width / 2),
                    - this.m_CanvasTransform.rect.height + m_ImageDialogBack.rectTransform.rect.height + 25
                );

            }

            this.m_ImageDialogBack.enabled = true;
        }

        IEnumerator _DisplayTextMessage(CharacterData character, string message, bool thought)
        {
            this.m_Message = string.IsNullOrEmpty(message) ? "..." : message;
            this.m_MessagePosition = 1;

            var handle = Addressables.LoadAssetAsync<AudioClip>($"sfx/blip-{character.DialogBlip.ToString()}");

            yield return handle;

            this.m_BlipAudioSource.clip = thought ? null : handle.Result;
            this.m_ImageForward.gameObject.SetActive(false);

            if (this.m_WriteTimeInterval == 0)
            {
                this.m_MessagePosition = this.m_Message.Length;
            }
            #if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.X))
                this.m_MessagePosition = this.m_Message.Length;
            #endif

            try
            {
                while (this.m_MessagePosition < this.m_Message.Length)
                {
                    char c = this.m_Message[this.m_MessagePosition];
                    this.m_IsRollingText  = !(c ==',' || c=='.' || c=='!' || c=='?');
                    this.m_BlipAudioSource.enabled = m_IsRollingText;
                    this.m_TextMessage.text = this.m_Message.Substring(0, this.m_MessagePosition++);
                    yield return new WaitForSeconds(this.m_WriteTimeInterval * (m_IsRollingText?1:4));
                }
            } 
            finally
            {
                this.m_IsRollingText = false;
                this.m_TextMessage.text = this.m_Message;
                this.m_BlipAudioSource.enabled = false;
                this.m_MessagePosition = this.m_Message.Length;
                this.m_ImageForward.gameObject.SetActive(true);
            }
        }

    }
}

