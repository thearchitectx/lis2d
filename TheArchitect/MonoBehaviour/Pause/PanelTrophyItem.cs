using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using TheArchitect.Core;

namespace TheArchitect.MonoBehaviour
{
        
    public class PanelTrophyItem : UnityEngine.MonoBehaviour
    {
        
        [SerializeField] public Image ImageBG;
        [SerializeField] public Image ImageTrophy;
        [SerializeField] public Text TextLabel;
        [SerializeField] public Text TextDescription;
        [SerializeField] public Text TextUnlock;
        [SerializeField] public Material MaterialLockedImage;

        void Start()
        {
            ImageTrophy.color = new Color(1, 1, 1, 0);
        }

        public void SetTrophy(Trophy trophy)
        {
            TextLabel.text = trophy.Label;
            TextLabel.color = trophy.Unlocked ? Color.white : Color.gray;
            TextDescription.text = trophy.Description;
            TextDescription.color = trophy.Unlocked ? Color.white : Color.gray;
            TextUnlock.text = trophy.Unlocked ? $"Unlocked {trophy.GetUnlockDate()}" : "";
            
            Addressables.LoadAssetAsync<Sprite>(trophy.Icon).Completed += (handle) => {
                ImageTrophy.sprite = handle.Result;
                ImageTrophy.preserveAspect = true;
                if (!trophy.Unlocked)
                {
                    ImageTrophy.material = MaterialLockedImage;
                }
            };
        }

        void Update()
        {
            if (this.ImageTrophy.sprite != null)
                this.ImageTrophy.color = new Color(1,1,1, this.ImageTrophy.color.a + Time.unscaledDeltaTime);
        }

        void OnDestroy()
        {
            Addressables.Release(ImageTrophy.sprite);
        }

    }

}