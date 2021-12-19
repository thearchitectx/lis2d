using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;
using TheArchitect.MonoBehaviour;

namespace TheArchitect.XMLScript.Action
{
    public class TrophyAction : XMLScriptAction
    {
        [XmlAttribute("unlock")]
        public string Id;

        public override string Update(XMLScriptInstance xmlscript, XMLScriptController controller)
        {
            var trophy = Trophies.Get(Id);
            if (trophy.Unlock())
            {
                Addressables.InstantiateAsync("trophy-notification.prefab").Completed += (handle) => {
                    PanelTrophyItem panel = handle.Result.GetComponentInChildren<PanelTrophyItem>();
                    panel.SetTrophy(trophy);
                    panel.ImageBG.sprite = null;
                    panel.ImageBG.color = new Color32(0x3d, 0x3d, 0x3d, 0xFF);
                };
            }

            return OUTPUT_NEXT;
        }

    }
}