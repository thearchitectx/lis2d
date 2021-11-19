using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TheArchitect.Core;
using TheArchitect.Game;

namespace TheArchitect.MonoBehaviour.Save
{

    struct SaveData
    {
        public bool CanSave;
        public string Slot;
        public string Label;
        public string Date;
        public byte[] Screen;
    } 

    public class PanelSaveIO : UnityEngine.MonoBehaviour
    {
        public const int SAVE_SLOTS = 0;
        public GameObject PanelSaveItemPrefab;
        public RectTransform TransformItemParent;
        public Text TextAllowed;
        public Button ButtonBack;
        private GameContext m_Context;

        public GameContext Context
        {
            set { this.m_Context = value; }
            get { return this.m_Context; }
        }

        void Start()
        {
            ReadData();
        }

        private void ReadData()
        {
            if (this.m_Context == null)
                return;

            string rootPath = Application.persistentDataPath;

            foreach (Transform t in TransformItemParent)
                Destroy(t.gameObject);

            var worker = new System.ComponentModel.BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, args) => {
                string[] saveSlotsFolders = new string[SAVE_SLOTS+1];
                saveSlotsFolders[0] = "autosave";
                for (var i = SAVE_SLOTS+1; i < SAVE_SLOTS+1; i++)
                    saveSlotsFolders[i] = $"{i:00}";

                for (var i = 0; i < SAVE_SLOTS+1; i++)
                {
                    SaveData d = new SaveData() {
                        Slot = saveSlotsFolders[i],
                        Label = GameStateIO.LoadLabel(rootPath, saveSlotsFolders[i]),
                        Date = GameStateIO.LoadLastWrite(rootPath, saveSlotsFolders[i]),
                        Screen = GameStateIO.LoadImage(rootPath, saveSlotsFolders[i]),
                        CanSave = i > 0
                    };
                    worker.ReportProgress(i*100 / 11, d);
                }
            };

            worker.ProgressChanged += (sender, progress) => {
                SaveData saveData = (SaveData) progress.UserState;

                PanelSaveItem item = Instantiate(PanelSaveItemPrefab).GetComponent<PanelSaveItem>();
                item.transform.SetParent(this.TransformItemParent, false);
                item.ImageScreen.color = Color.black;
                if (saveData.Screen.Length>0)
                {
                    var t = new Texture2D(256, 256, TextureFormat.RGB24, false);
                    t.LoadImage(saveData.Screen);
                    item.ImageScreen.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(t.width/2, t.height/2));
                    item.ImageScreen.color = Color.white;
                }
                item.TextSlot.text = saveData.Slot;
                item.TextLabel.text = !string.IsNullOrEmpty(saveData.Label) ? saveData.Label : "-- EMPTY --";
                item.TextDate.text = !string.IsNullOrEmpty(saveData.Date) ? saveData.Date : "--";

                item.InputLabel.onEndEdit.AddListener( value => {
                    item.InputLabel.gameObject.SetActive(false);
                    item.TextLabel.gameObject.SetActive(true);
                    GameStateIO.Rename(rootPath, saveData.Slot, value);
                    ReadData();
                });

                item.ButtonSave.gameObject.SetActive( saveData.CanSave );
                // item.ButtonSave.onClick.AddListener(() => StartCoroutine(DoSave(rootPath, slot, label)));

                item.ButtonLoad.gameObject.SetActive(saveData.Label != null);
                item.ButtonLoad.onClick.AddListener(() => DoLoad(rootPath, saveData.Slot));

                item.ButtonRename.gameObject.SetActive(saveData.Slot!="autosave" && saveData.Label != null);

                item.ButtonRename.onClick.AddListener( () => {
                    item.InputLabel.gameObject.SetActive(true);
                    item.InputLabel.text = item.TextLabel.text;
                    item.TextLabel.gameObject.SetActive(false);
                });

                item.ButtonDelete.gameObject.SetActive(saveData.Slot!="autosave" && saveData.Label != null);
                item.ButtonDelete.onClick.AddListener( () => {
                    const string TEXT_CONFIRM = "CONFIRM?";
                    Text btText = item.ButtonDelete.GetComponentInChildren<Text>();
                    if (btText.text==TEXT_CONFIRM)
                    {
                        string path = GameStateIO.GetSlotPath(rootPath, saveData.Slot);
                        Directory.Delete(path, true);
                        ReadData();
                    }
                    else
                        btText.text = TEXT_CONFIRM;
                });
            };
            
            worker.RunWorkerAsync();
        }

        private void DoLoad(string root, string slot)
        {
            var state = GameStateIO.Load(root, slot);
            this.m_Context.ApplyStateInstance(state);
            UnityEngine.SceneManagement.SceneManager.LoadScene("XMLScriptScene");
        }

        // private IEnumerator DoSave(string root, string slot, string label)
        // {
        //     // Save state
        //     m_Context.State.Save(root, slot, label != null ? label : $"SAVE SLOT {slot}", Application.version);

        //     // Prepare for screenshot
        //     yield return TakeSaveScreenshot(root, slot);

        //     ReadData();
        // }

        // public static IEnumerator TakeSaveScreenshot(string root, string slot)
        // {
        //     List<Canvas> allCanvas = GameObject.FindObjectsOfType<Canvas>().Where( c => c.enabled ).ToList();
        //     allCanvas.ForEach( c => c.enabled = false );
        //     PostProcessLayer ppl = GameObject.FindObjectOfType<PostProcessLayer>();
        //     if (ppl!=null)
        //         ppl.enabled = false;

        //     yield return new WaitForEndOfFrame();
            
        //     Texture2D texScreen = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //     try {
        //         texScreen.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        //         texScreen.Apply();
        //         texScreen = TextureScaler.scaled(texScreen, 256, Mathf.CeilToInt(256f / Screen.width * Screen.height));
        //     } finally {
        //         // Recover from screenshot
        //         if (ppl!=null) ppl.enabled = true;
        //         allCanvas.ForEach( c => c.enabled = true );
        //     }

        //     // Write screenshot
        //     File.WriteAllBytes( $"{GameState.GetSlotPath(root, slot)}/{GameState.SCREEN_FILE_NAME}" , texScreen.EncodeToJPG(80));
        // }

    }
}