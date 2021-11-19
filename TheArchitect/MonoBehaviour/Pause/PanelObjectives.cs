using UnityEngine;
using UnityEngine.UI;
using TheArchitect.Game;

namespace  TheArchitect.MonoBehaviour.Pause
{
    public class PanelObjectives : UnityEngine.MonoBehaviour
    {
        [SerializeField] private Transform m_Parent;
        [SerializeField] private Text m_TextDay;
        [SerializeField] private GameObject m_TextObjectivePrefab;
        [SerializeField] private GameObject m_TextObjectiveDescriptionPrefab;
        [SerializeField] private GameContext m_Game;

        private string[] m_Days = {"MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY"};
        // Start is called before the first frame update
        // void Start()
        // {
        //     int day = Mathf.Clamp(this.m_Game.GetFlagState("DATA_CURRENT_DAY"), 1, int.MaxValue);
        //     this.m_TextDay.text = m_Days[Mathf.Clamp(day-1, 0, m_Days.Length-1) ];

        //     foreach (var objective in this.m_Game.GetObjectives())
        //     {
        //         if (objective!=null)
        //         {
        //             var textTitle = GameObject.Instantiate(this.m_TextObjectivePrefab).GetComponent<Text>();
        //             textTitle.text = $"- {objective.Title}";
        //             textTitle.transform.SetParent(this.m_Parent, false);

        //             var textDescription = GameObject.Instantiate(this.m_TextObjectiveDescriptionPrefab).GetComponent<Text>();
        //             textDescription.text = objective.Description;
        //             textDescription.transform.SetParent(this.m_Parent, false);
        //         }
        //     }
        // }

    }

}
