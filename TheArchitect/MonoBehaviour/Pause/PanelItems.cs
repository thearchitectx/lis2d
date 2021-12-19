using System.Linq;
using UnityEngine;
using TheArchitect.Game;
using TheArchitect.Core;

namespace TheArchitect.MonoBehaviour.Pause
{

    public class PanelItems : UnityEngine.MonoBehaviour
    {
        [SerializeField] public GameObject ItemPrefab;
        [SerializeField] public GameContext m_Context;

        void Start()
        {
            foreach (Transform t in this.transform)
                    Destroy(t.gameObject);
                    
            Items.GetItems().ToList()
                .Where( item => this.m_Context.GetVariable($"ITEM:{item.Id}", 0) > 0 )
                .ToList()
                .ForEach( itemData => {
                    PanelItem pi =  Instantiate(ItemPrefab, this.transform, false).GetComponent<PanelItem>();
                    pi.TextLabel.text = itemData.Label;
                    var qtd = this.m_Context.GetVariable($"ITEM:{itemData.Id}");
                    pi.TextValue.text = $"x{qtd}";
                    pi.SetIconKey(itemData.Icon);
                });
            
                
        }
    }

}
