using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TheArchitect.Game;

namespace TheArchitect.MonoBehaviour.Save
{

    struct SaveData
    {
        public string Slot;
        public string Label;
        public string Date;
        public byte[] Screen;
    } 

    public class PanelSaveIO : UnityEngine.MonoBehaviour
    {
        public const int SAVE_SLOTS = 20;
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

        private static string SlotAsText(int slot)
        {

            return slot == 0 ? GameStateIO.AUTOSAVE_SLOT : $"{slot:00}";
        }

        private void ReadData()
        {
            string rootPath = Application.persistentDataPath;

            foreach (Transform t in TransformItemParent)
                Destroy(t.gameObject);

            var worker = new System.ComponentModel.BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, args) => {
                string[] saveSlotsFolders = new string[SAVE_SLOTS+1];
                for (var i = 0; i < SAVE_SLOTS; i++)
                    saveSlotsFolders[i] = SlotAsText(i);

                for (var i = 0; i < SAVE_SLOTS; i++)
                {
                    SaveData d = new SaveData() {
                        Slot = saveSlotsFolders[i],
                        Label = GameStateIO.LoadLabel(rootPath, saveSlotsFolders[i]),
                        Date = GameStateIO.LoadLastWrite(rootPath, saveSlotsFolders[i]),
                        Screen = GameStateIO.LoadImage(rootPath, saveSlotsFolders[i]),
                    };
                    worker.ReportProgress(i*100 / SAVE_SLOTS, d);
                }
            };

            worker.ProgressChanged += (sender, progress) => {
                SaveData saveData = (SaveData) progress.UserState;

                PanelSaveItem item = Instantiate(PanelSaveItemPrefab).GetComponent<PanelSaveItem>();
                item.transform.SetParent(this.TransformItemParent, false);
                
                item.IsCurrentItem = saveData.Slot == GameStateIO.AUTOSAVE_SLOT;

                item.ImageScreen.color = Color.black;
                if (saveData.Screen.Length>0)
                {
                    var t = new Texture2D(256, 256, TextureFormat.RGB24, false);
                    t.LoadImage(saveData.Screen);
                    item.ImageScreen.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(t.width/2, t.height/2));
                    item.ImageScreen.color = Color.white;
                }
                item.TextSlot.text = saveData.Slot == GameStateIO.AUTOSAVE_SLOT ? "AUTOSAVE" : saveData.Slot;
                item.TextLabel.text = !string.IsNullOrEmpty(saveData.Label) ? saveData.Label : "-- EMPTY --";
                item.TextDate.text = !string.IsNullOrEmpty(saveData.Date) ? saveData.Date : "--";

                item.InputLabel.onEndEdit.AddListener( value => {
                    item.InputLabel.gameObject.SetActive(false);
                    item.TextLabel.gameObject.SetActive(true);
                    GameStateIO.Rename(rootPath, saveData.Slot, value);
                    ReadData();
                });

                item.ButtonSwitch.gameObject.SetActive(!item.IsCurrentItem);

                item.ButtonSwitch.onClick.AddListener( () => {
                    const string TEXT_CONFIRM = "OVERWRITE?";
                    Text btText = item.ButtonSwitch.GetComponentInChildren<Text>();
                    string from = "autosave";
                    if (saveData.Label == null || btText.text==TEXT_CONFIRM)
                    {
                        GameStateIO.CopySlot(rootPath, from, saveData.Slot);
                        ReadData();
                    }
                    else
                        btText.text = TEXT_CONFIRM;
                });


                item.ButtonLoad.interactable = saveData.Label != null;
                item.ButtonLoad.GetComponentInChildren<Text>().text = saveData.Label == null ? "" : "LOAD";
                item.ButtonLoad.onClick.AddListener(() => DoLoad(rootPath, saveData.Slot));


                item.ButtonRename.interactable = saveData.Label != null;
                item.ButtonRename.gameObject.SetActive(!item.IsCurrentItem);
                item.ButtonRename.GetComponentInChildren<Text>().text = saveData.Label == null ? "" : "RENAME";
                item.ButtonRename.onClick.AddListener( () => {
                    item.InputLabel.gameObject.SetActive(true);
                    item.InputLabel.text = item.TextLabel.text;
                    item.TextLabel.gameObject.SetActive(false);
                });

                item.ButtonDelete.interactable = saveData.Label != null;
                item.ButtonDelete.gameObject.SetActive(!item.IsCurrentItem);
                item.ButtonDelete.GetComponentInChildren<Text>().text = saveData.Label == null ? "" : "DELETE";
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

    }
}