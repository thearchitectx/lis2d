using UnityEngine.UI;
using UnityEngine;
using TheArchitect.Core;

namespace TheArchitect.MonoBehaviour.Pause
{

    public class PanelCharacter : UnityEngine.MonoBehaviour
    {
        [SerializeField] public GameObject PanelStatPrefab;
        [SerializeField] public Image ImageCharacter;
        [SerializeField] public Text TextProfile;
        [SerializeField] public Text TextIntel;
        [SerializeField] public Transform PanelStats;

        // public void SetCharacter(Game game, Character character)
        // {
        //     this.ImageCharacter.sprite = character.Sprite;
        //     this.ImageCharacter.color = Color.white;

        //     foreach (Transform t in this.PanelStats.transform)
        //         Destroy(t.gameObject);
            
        //     this.TextIntel.text = "";
        //     for (int i=0; i < character.Intel.Length; i++)
        //     {
        //         this.TextIntel.text += game.GetCharacterStat(character, string.Format(Character.STAT_INTEL_, i)) > 0
        //             ? $"- {character.Intel[i]}\n\n"
        //             : "- ????\n\n";
        //     }

        //     this.TextProfile.text = $"<color=cyan>AGE</color>\n{character.Age}\n\n<color=cyan>STATUS</color>\nACQUAINTANCE";

        //     AddStat(game, character, Character.STAT_AFFINITY, "AFFINITY", false, 0, 0);
        //     AddStat(game, character, Character.STAT_CORRUPTION, "CORRUPTION", false, 0, 0);
        //     AddStat(game, character, Character.STAT_VICTORIA_LOYALTY, "VICTORIA'S INFLUENCE", false, 3, 3);
        // }

        // private void AddStat(Game game, Character character, string stat, string label, bool force, byte stars, int complement)
        // {
        //     if ( game.HasCharacterStat(character, stat) || force)
        //     {
        //         int s = game.GetCharacterStat(character, stat);
        //         PanelStat panelStat = Instantiate(PanelStatPrefab).GetComponent<PanelStat>();
        //         panelStat.transform.SetParent(PanelStats, false);

        //         panelStat.TextLabel.text = label;
        //         panelStat.TextValue.text = s.ToString("D2");
        //         panelStat.TextValue.gameObject.SetActive(stars == 0);

        //         for (var i=0; i<Mathf.Min(stars,5); i++)
        //         {
        //             var star = GameObject.Instantiate(panelStat.ImageStar.gameObject);
        //             star.transform.SetParent(panelStat.ImageStar.transform.parent, false);
        //             star.SetActive(true);
        //             star.GetComponent<Image>().color = s > i ? new Color32(0xCA, 0xC4, 0x0C, 0xff) : new Color32(0x9a, 0x9a, 0x9a, 0x9a);
        //         }
        //         Destroy(panelStat.ImageStar.gameObject);
        //     }
        // }

    }

}
