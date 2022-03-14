using  TheArchitect.MonoBehaviour.Save;
using  TheArchitect.Game;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace TheArchitect.MonoBehaviour.Title
{
    public class Title : UnityEngine.MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameContext m_ContextReference;
        [SerializeField] private AssetReferenceGameObject m_PanelSavePrefab;
        [SerializeField] private AssetReferenceGameObject m_PanelTrophiesPrefab;
        [SerializeField] private Transform m_Root;
        [SerializeField] private TitleSettings m_Settings;
        [SerializeField] private Text m_TextVersion;

        private GameContext m_Context;

        void Start()
        {
            Time.timeScale = 1;
            this.m_TextVersion.text = Application.version;
            this.m_ContextReference.LoadAssetAsync().Completed += handle => {
                this.m_Context = handle.Result;
            };

            this.m_Settings.onApply.AddListener(() => {
                this.m_Settings.gameObject.SetActive(false);
                this.m_Root.gameObject.SetActive(true);
            });
        }

        public void OpenTheArchitect()
        {
            Application.OpenURL("https://www.patreon.com/the_architect");
        }

        public void MenuQuit()
        {
            Application.Quit();
        }

        public void MenuLoad()
        {
            this.m_Root.gameObject.SetActive(false);
            this.m_PanelSavePrefab.InstantiateAsync().Completed += handle => {
                handle.Result.GetComponent<Canvas>().sortingOrder = 1000;
                handle.Result.GetComponentInChildren<PanelSaveIO>().Context = this.m_Context;
                handle.Result.GetComponentInChildren<PanelSaveIO>().ButtonBack.onClick.AddListener(
                    () => {
                        this.m_Root.gameObject.SetActive(true);
                        this.m_PanelSavePrefab.ReleaseInstance(handle.Result);
                    }
                );

            };
        }

        public void MenuTrophies()
        {
            this.m_Root.gameObject.SetActive(false);
            this.m_PanelTrophiesPrefab.InstantiateAsync().Completed += handle => {
                handle.Result.GetComponent<Canvas>().sortingOrder = 1000;
                handle.Result.GetComponentInChildren<PanelTrophies>().ButtonBack.onClick.AddListener(
                    () => { 
                        this.m_Root.gameObject.SetActive(true);
                        this.m_PanelTrophiesPrefab.ReleaseInstance(handle.Result);
                    }
                );

            };
        }

        public void MenuSettings()
        {
            this.m_Root.gameObject.SetActive(false);
            this.m_Settings.gameObject.SetActive(true);
            this.m_Settings.ReadFromPrefs();
        }

        public void MenuStart()
        {
            this.m_Context.ApplyStateInstance(new GameState());
            
            var slot = PanelSaveIO.FirstFreeSlot();
            if (string.IsNullOrEmpty(slot))
            {
                slot = "01";
                this.m_Context.SetVariable("FLG:0", 1);
            }
            this.m_Context.SetVariable(GameState.SYSTEM_SAVE_SLOT, slot);

            Debug.Log($"Starting new game on slot {slot}");
            UnityEngine.SceneManagement.SceneManager.LoadScene("XMLScriptScene");
        }

    }
}