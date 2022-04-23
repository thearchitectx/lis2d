using System;
using TheArchitect.SceneObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace TheArchitect.MonoBehaviour
{
    public class PlayerLaptopUI : SceneObject
    {
        [SerializeField] private ScrollRect m_ScrollRect;
        [SerializeField] private Text m_TextUrl;
        [SerializeField] private Text m_TextContent;
        [SerializeField] private Image m_ImageContent;

        void OnDestroy()
        {
            if (this.m_ImageContent.sprite != null)
                Addressables.Release(this.m_ImageContent.sprite);
        }

        public void SetTextURL(string text)
        {
            this.m_TextUrl.text = text;
        }

        public void SetTextContent(string text)
        {
            if (this.m_ImageContent.sprite != null)
                Addressables.Release(this.m_ImageContent.sprite);

            this.m_ImageContent.sprite = null;
            this.m_ImageContent.gameObject.SetActive(false);
            this.m_TextContent.text = text;

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_TextContent.transform.parent.GetComponent<RectTransform>());

            this.m_ScrollRect.verticalScrollbar.value = 1;
        }

        public void SetImageContent(string sprite)
        {
            if (this.m_ImageContent.sprite != null)
                Addressables.Release(this.m_ImageContent.sprite);

            Addressables.LoadAssetAsync<Sprite>(sprite).Completed += h => {
                this.m_ImageContent.gameObject.SetActive(h.Result != null);
                this.m_ImageContent.sprite = h.Result;
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_TextContent.transform.parent.GetComponent<RectTransform>());
            };
        }

        public void SetTextContentAlign(string text)
        {
            this.m_TextContent.alignment = (TextAnchor) System.Enum.Parse(typeof(TextAnchor), text) ;
        }
    }  
}
