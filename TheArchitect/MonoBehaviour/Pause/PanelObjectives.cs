using UnityEngine;
using UnityEngine.UI;
using TheArchitect.Game;
using System.Linq;

namespace  TheArchitect.MonoBehaviour.Pause
{
    public class PanelObjectives : UnityEngine.MonoBehaviour
    {
        [SerializeField] private Transform m_Parent;
        [SerializeField] private Text m_TextDay;
        [SerializeField] private GameObject m_TextObjectivePrefab;
        [SerializeField] private GameContext m_Game;

        private string[] m_Days = {"MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY"};

        // Start is called before the first frame update
        void Start()
        {
            int day = Mathf.Clamp(this.m_Game.GetVariable("DAY:CURRENT", 1), 1, int.MaxValue);
            this.m_TextDay.text = m_Days[Mathf.Clamp(day-1, 0, m_Days.Length-1) ];

            this.m_Game.GetVariableNames()
                .Where( v => v.StartsWith("QUEST:") && v.EndsWith(":OBJECTIVE") )
                .ToList()
                .ForEach( v => {
                    var textDescription = GameObject.Instantiate(this.m_TextObjectivePrefab, this.m_Parent, false).GetComponentInChildren<Text>();
                    textDescription.text = this.m_Game.GetVariable(v, "");
                });

        }

    }

}
