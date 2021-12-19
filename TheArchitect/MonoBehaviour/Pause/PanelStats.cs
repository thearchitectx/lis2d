using UnityEngine.UI;
using UnityEngine;
using TheArchitect.Game;

namespace TheArchitect.MonoBehaviour.Pause
{

    public class PanelStats : UnityEngine.MonoBehaviour
    {
        [SerializeField] public Text m_TextPlaytime;
        [SerializeField] public Image m_ImageFillParagon;
        [SerializeField] public Text m_TextParagonScore;
        [SerializeField] public Image m_ImageFillRenegade;
        [SerializeField] public Text m_TextRenegadeScore;

        [SerializeField] private GameContext m_Game;

        private float m_MaxReputation = 30;
        private int m_RenegadeScore = 0;
        private int m_ParagonScore = 0;

        void Start()
        {
            this.m_ParagonScore = this.m_Game.GetVariable("PLAYER:STAT:PARAGON", 0);
            this.m_RenegadeScore = this.m_Game.GetVariable("PLAYER:STAT:RENEGADE", 0);

            this.m_TextParagonScore.text = $"{this.m_ParagonScore:D2}";
            this.m_TextRenegadeScore.text = $"{this.m_RenegadeScore:D2}";

            this.m_ImageFillParagon.fillAmount = 0;
            this.m_ImageFillRenegade.fillAmount = 0;
        }

        void Update()
        {
            var t = this.m_Game.PlayTime;
            int sec = (int) (t%60);
            int minutes = (int) ((t/60)%60);
            int hours = (int) ((t/3600)%24);

            this.m_TextPlaytime.text = $"PLAYTIME: {hours:D2}:{minutes:D2}:{sec:D2}";

            this.m_ImageFillParagon.fillAmount = Mathf.MoveTowards(this.m_ImageFillParagon.fillAmount, this.m_ParagonScore / this.m_MaxReputation, Time.unscaledDeltaTime);
            this.m_ImageFillRenegade.fillAmount = Mathf.MoveTowards(this.m_ImageFillRenegade.fillAmount, this.m_RenegadeScore / this.m_MaxReputation, Time.unscaledDeltaTime);
        }
    }

}
