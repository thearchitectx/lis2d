using UnityEngine.UI;
using UnityEngine;
using TheArchitect.Core;

namespace TheArchitect.MonoBehaviour.Pause
{

    public class PanelStats : UnityEngine.MonoBehaviour
    {
        [SerializeField] public PanelStatCategory PanelDICK;
        [SerializeField] public PanelStatCategory PanelPERKS;

        // void Start()
        // {
        //     Game game = Resources.Load<Game>(ResourcePaths.SO_GAME);
        //     this.PanelDICK.AddStat("DARING", game.GetFlagState("DICK_DARING").ToString("D2"));
        //     this.PanelDICK.AddStat("INTELLIGENCE", game.GetFlagState("DICK_INTELLIGENCE").ToString("D2"));
        //     this.PanelDICK.AddStat("CHARISMA", game.GetFlagState("DICK_CHARISMA").ToString("D2"));
            
        //     int karma = game.GetFlagState("DICK_KARMA");
        //     this.PanelDICK.AddStat("KARMA", $"<color=#{(karma>=0?"0500FF":"AA0005")}>{Mathf.Abs(karma).ToString("D2")}</color>"); // FF0013

        //     foreach (Perk perk in game.GetActivePerks())
        //     {
        //         this.PanelPERKS.AddPerk(perk);
        //     }
        // }
    }

}
