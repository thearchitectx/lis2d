using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TheArchitect.MonoBehaviour.Pause
{

    public class PanelItem : UnityEngine.MonoBehaviour
    {
        [SerializeField] public Image ImageIcon;
        [SerializeField] public Text TextLabel;
        [SerializeField] public Text TextValue;

        void Start()
        {
            this.ImageIcon.color = new Color(1,1,1,0);
        }

        public void SetIconKey(string icon)
        {
            if (!string.IsNullOrEmpty(icon))
            {
                Addressables.LoadAssetAsync<Sprite>(icon).Completed += handle => {
                    this.ImageIcon.sprite = handle.Result;
                };
            }
        }

        void Update()
        {
            if (this.ImageIcon.sprite != null && this.ImageIcon.color.a < 1)
                this.ImageIcon.color += new Color(0, 0, 0, Time.unscaledDeltaTime);
        }

        void OnDestroy()
        {
            if (this.ImageIcon.sprite != null)
                Addressables.Release(this.ImageIcon.sprite);
        }
    }

}
