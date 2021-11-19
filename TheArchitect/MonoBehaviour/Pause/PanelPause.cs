using UnityEngine.UI;
using UnityEngine;
using TheArchitect.Game;

namespace TheArchitect.MonoBehaviour.Pause
{

    public class PanelPause : UnityEngine.MonoBehaviour
    {
        [SerializeField] public Toggle ButtonStats;
        [SerializeField] public Toggle ButtonItems;
        [SerializeField] public Toggle ButtonCharacters;
        [SerializeField] public Toggle ButtonObjectives;
        [SerializeField] public Button ButtonQuit;
        [SerializeField] public Button ButtonLoadSave;
        [SerializeField] public Button ButtonBack;
        [SerializeField] public RectTransform PanelContent;

        [SerializeField] private GameObject PanelStatsPrefab;
        [SerializeField] private GameObject PanelItemsPrefab;
        [SerializeField] private GameObject PanelCharactersPrefab;
        [SerializeField] private GameObject PanelObjectivesPrefab;

        public GameContext Context;

        void Start()
        {
            RefreshContent();
        }

        public void RefreshContent()
        {
            foreach (Transform t in this.PanelContent)
                Destroy(t.gameObject);
            
            GameObject g = null;
            if (ButtonStats.isOn)
                g = Instantiate(PanelStatsPrefab);
            else if (ButtonCharacters.isOn)
                g = Instantiate(PanelCharactersPrefab);
            else if (ButtonItems.isOn)
                g = Instantiate(PanelItemsPrefab);
            else if (ButtonObjectives.isOn)
                g = Instantiate(PanelObjectivesPrefab);

            if (g!=null)
                g.transform.SetParent(PanelContent, false);
        }

        public void Quit()
        {
            const string CONFIRM = "CONFIRM?";
            
            Text t = ButtonQuit.GetComponentInChildren<Text>();
            if (t.text == CONFIRM)
                UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
            else
                t.text = CONFIRM;
        }

        public void Back()
        {
            this.transform.GetComponentInParent<PauseController>().Close();
        }

        public void LoadSave()
        {
            this.transform.GetComponentInParent<PauseController>().ToggleLoadSave();
        }

        public void Trophies()
        {
            this.transform.GetComponentInParent<PauseController>().ToggleTrophies();
        }

    }

}
