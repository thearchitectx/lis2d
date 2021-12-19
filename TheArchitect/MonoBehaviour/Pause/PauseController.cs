using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.AddressableAssets;
using TheArchitect.Game;
using TheArchitect.MonoBehaviour.Pause;
using TheArchitect.MonoBehaviour.Save;

namespace TheArchitect.MonoBehaviour.Pause
{

    public class PauseController : UnityEngine.MonoBehaviour
    {
        public const string PLAYER_PREF_SHOW_FPS = "PauseController.SHOW_FPS";
        public const string PLAYER_PREF_HIDE_OBJECTIVES = "PauseController.HIDE_OBJECTIVES";
        public const string PLAYER_PREF_IGNORE_EXCEPTION = "PauseController.PLAYER_PREF_IGNORE_EXCEPTION";

        [SerializeField] public PanelPause PanelPausePrefab;
        [SerializeField] public PanelSaveIO PanelSavePrefab;
        [SerializeField] public GameObject CanvasExceptionPrefab;
        [SerializeField] public GameObject CanvasTrophiesPrefab;
        [SerializeField] public GameObject CanvasFPSPrefab;
        [SerializeField] private Volume m_Volume;
        [SerializeField] private AssetReferenceGameContext m_ContextAsset;
        
        private GameContext m_Context;
        private PanelPause m_PanelPause;
        private PanelSaveIO m_PanelSave;
        private Transform m_CanvasFPS;
        private Transform m_CanvasTrophies;
        private PanelException m_PanelException;
        private Transform m_CanvasObjective;
        private Canvas[] m_DisabledCanvases;

        private bool m_IgnoreException;

        void Start()
        {
            Time.timeScale = 1;
            this.m_ContextAsset.LoadAssetAsync().Completed += (handle) => this.m_Context = handle.Result;

            if (PlayerPrefs.GetInt(PLAYER_PREF_SHOW_FPS, 0) == 1)
                ShowFPS();

            this.m_IgnoreException = PlayerPrefs.GetInt(PLAYER_PREF_IGNORE_EXCEPTION, 0) == 1;
        }

        void OnDestroy() {
            this.m_Context.SetVariable(GameState.SYSTEM_PLAYTIME, this.m_Context.GetVariable(GameState.SYSTEM_PLAYTIME, 0f) + Time.timeSinceLevelLoad );
        }

        // void OnDisable()
        // {
        //     Application.logMessageReceived -= HandleException;
        // }

        // void OnEnable()
        // {
        //     Application.logMessageReceived += HandleException;
        // }

        void Update()
        {
            if (this.m_Context == null)
                return; 

            if (Input.GetButtonDown("Options"))
            {
                if (this.m_PanelPause == null)
                    Open();
                else
                    Close();
            }

            if (m_PanelPause == null && Input.GetKeyDown(KeyCode.F12))
            {
                // this.m_IgnoreException = !this.m_IgnoreException;
                // PlayerPrefs.SetInt(PLAYER_PREF_IGNORE_EXCEPTION, this.m_IgnoreException ? 1 : 0);
                // PlayerPrefs.Save();
                // Resources.Load<TheArchitect.Core.Data.Variables.Console>(ResourcePaths.SO_CONSOLE).Log("EXCEPTION_DIALOG", "Exception notification "+ (this.m_IgnoreException?"disabled":"enabled"));
            }
            if (m_PanelPause == null && Input.GetKeyDown(KeyCode.F11))
            {
                ToggleFPS();
            }

            this.m_Volume.weight = Mathf.Clamp01(this.m_Volume.weight + Time.unscaledDeltaTime * 2 * (this.m_PanelPause == null ? -1 : 1));
            this.m_Volume.enabled = this.m_Volume.weight > 0;
        }

        public void Open()
        {
            this.m_DisabledCanvases = GameObject.FindObjectsOfType<Canvas>();
            foreach (Canvas c in this.m_DisabledCanvases)
                c.enabled = false;

            this.m_PanelPause = Instantiate(this.PanelPausePrefab.gameObject).GetComponent<PanelPause>();
            this.m_PanelPause.transform.SetParent(this.transform, false);
            this.m_PanelPause.Context = this.m_Context;
            Time.timeScale = 0;
        }

        public void Close()
        {
            if (this.m_PanelSave!=null)
            {
                Destroy(this.m_PanelSave.gameObject);
                this.m_PanelSave = null;
            }

            if (this.m_CanvasTrophies!=null)
            {
                Destroy(this.m_CanvasTrophies.gameObject);
                this.m_CanvasTrophies = null;
            }
            
            Destroy(this.m_PanelPause.gameObject);
            this.m_PanelPause = null;

            Time.timeScale = 1;

            foreach (Canvas c in this.m_DisabledCanvases)
                if (c!=null) c.enabled = true;

            this.m_DisabledCanvases = null;
        }

        public void ToggleFPS()
        {
            if (m_CanvasFPS==null)
                ShowFPS();
            else
                HideFPS();
        }

        public void ShowFPS()
        {
            m_CanvasFPS = GameObject.Instantiate(CanvasFPSPrefab).transform;
            m_CanvasFPS.SetParent(this.transform);
            PlayerPrefs.SetInt(PLAYER_PREF_SHOW_FPS, 1);
        }

        public void HideFPS()
        {
            Destroy(m_CanvasFPS.gameObject);
            PlayerPrefs.SetInt(PLAYER_PREF_SHOW_FPS, 0);
        }



        public void ToggleLoadSave()
        {
            if (this.m_PanelPause.gameObject.activeSelf)
            {
                this.m_PanelSave = Instantiate(this.PanelSavePrefab).GetComponent<PanelSaveIO>();
                this.m_PanelSave.transform.SetParent(this.transform, false);
                this.m_PanelSave.ButtonBack.onClick.AddListener(() => ToggleLoadSave());
                this.m_PanelSave.Context = this.m_Context;
                this.m_PanelSave.gameObject.SetActive(true);
                this.m_PanelPause.gameObject.SetActive(false);
            }
            else
            {
                Destroy(this.m_PanelSave.gameObject);
                this.m_PanelPause.gameObject.SetActive(true);
            }

        }

        public void ToggleTrophies()
        {
            if (this.m_CanvasTrophies == null)
            {
                this.m_CanvasTrophies = Instantiate(this.CanvasTrophiesPrefab).GetComponent<Transform>();
                this.m_CanvasTrophies.SetParent(this.transform, false);
                this.m_CanvasTrophies.GetComponentInChildren<PanelTrophies>()
                    .ButtonBack.onClick.AddListener(() => ToggleTrophies()
                );
                this.m_PanelPause.gameObject.SetActive(false);
            }
            else
            {
                Destroy(this.m_CanvasTrophies.gameObject);
                this.m_PanelPause.gameObject.SetActive(true);
            }

        }

        private void HandleException(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception && this.m_PanelException == null && !this.m_IgnoreException)
            {
                Time.timeScale = 0;
                this.m_PanelException = Instantiate(CanvasExceptionPrefab).GetComponentInChildren<PanelException>();
                this.m_PanelException.transform.SetParent(this.transform, false);
                this.m_PanelException.TextException.text = condition + "\n"+ stackTrace;
                this.m_PanelException.ButtonWhatever.onClick.AddListener(() => {
                    Destroy(this.m_PanelException.gameObject);
                    Time.timeScale = 1;
                });
                this.m_PanelException.ButtonStopShowing.onClick.AddListener(() => {
                    PlayerPrefs.SetInt(PLAYER_PREF_IGNORE_EXCEPTION, 1);
                    Destroy(this.m_PanelException.gameObject);
                    Time.timeScale = 1;
                });
                this.m_PanelException.ButtonCopy.onClick.AddListener( () => {
                    GUIUtility.systemCopyBuffer = this.m_PanelException.TextException.text;
                    this.m_PanelException.ButtonCopy.GetComponentInChildren<Text>().text = "COPIED!";
                });

            }
        }
    }

}
