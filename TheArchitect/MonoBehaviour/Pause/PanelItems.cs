using UnityEngine;

namespace TheArchitect.MonoBehaviour.Pause
{

    public class PanelItems : UnityEngine.MonoBehaviour
    {
        [SerializeField] public GameObject ItemPrefab;

        void Start()
        {
            // foreach (Transform t in this.transform)
            //     Destroy(t.gameObject);
                
            // Game game = Resources.Load<Game>(ResourcePaths.SO_GAME);
            // foreach (KeyValuePair<Item, int> item in game.GetInventory())
            // {
            //     PanelItem pi =  Instantiate(ItemPrefab).GetComponent<PanelItem>();
            //     pi.TextLabel.text = item.Key.LabelUpper;
            //     pi.TextValue.text = $"x{item.Value}";
            //     pi.ImageIcon.sprite = item.Key.Icon;
            //     pi.ImageIcon.color = Color.white;
            //     pi.transform.SetParent(this.transform, false);
            // }
        }
    }

}
