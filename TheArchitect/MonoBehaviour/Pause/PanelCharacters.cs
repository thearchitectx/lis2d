using UnityEngine;

namespace TheArchitect.MonoBehaviour.Pause
{

    public class PanelCharacters : UnityEngine.MonoBehaviour
    {
        [SerializeField] public GameObject ToggleCharacterPrefab;
        [SerializeField] public PanelCharacter PanelCharacter;

        // void Start()
        // {
        //     Game game = Resources.Load<Game>(ResourcePaths.SO_GAME);
        //     Character[] characters = Resources.LoadAll<Character>(ResourcePaths.SO_CHARACTERS);
        //     foreach (Character c in characters)
        //     {
        //         int rel = game.GetCharacterStat(c, Character.STAT_INTEL);
        //         if (rel > 0)
        //         {
        //             Toggle newButton = Instantiate(ToggleCharacterPrefab).GetComponent<Toggle>();
        //             newButton.transform.SetParent(this.transform, false);
        //             newButton.GetComponentInChildren<Text>().text = c.DisplayName;
        //             newButton.group = this.GetComponent<ToggleGroup>();
        //             newButton.onValueChanged.AddListener( isOn => {
        //                 if (isOn)
        //                     PanelCharacter.SetCharacter(game, c);
        //             });
        //         }
        //     }

        // }

    }

}
