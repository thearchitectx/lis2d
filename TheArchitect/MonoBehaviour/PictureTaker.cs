using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TheArchitect.Game;

namespace TheArchitect.SceneObjects
{
    public class PictureTaker : SceneObject
    {   
        public const string OUTCOME_DONE = "DONE";

        [SerializeField] public Image ImageFlash;
        [SerializeField] private AudioSource m_AudioSource;
        private string m_PicName;
        Texture2D tex;

        private float m_StartTime;
        private Coroutine m_SnapCoroutine;

        public void SetPictureName(string name)
        {
            this.m_PicName = name;
        }

        void Start()
        {
            this.m_StartTime = Time.time + 1;
            this.m_AudioSource = this.GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.deltaTime==0 || Time.time < this.m_StartTime)
                return;

            if (this.m_SnapCoroutine == null && Input.GetMouseButtonDown(0))
            {
                this.m_SnapCoroutine = StartCoroutine(CoroutineTakePic());
            }
        }
        
        [ContextMenu("Take Pic")]
        public void TakePic()
        {
            if (this.m_SnapCoroutine==null)
                this.m_SnapCoroutine = StartCoroutine(CoroutineTakePic());
        }

        IEnumerator CoroutineTakePic()
        {
            Canvas[] allCanvas = GameObject.FindObjectsOfType<Canvas>();
            foreach (var c in allCanvas)
                c.enabled = false;

            yield return new WaitForEndOfFrame();

            string name = this.m_PicName == null ? $"pic-{UnityEngine.Random.value*100000:F0}" : this.m_PicName;
            string path = $"{Application.persistentDataPath}/{GameStateIO.PICS_FOLDER}";
            Directory.CreateDirectory(path);
            path = $"{path}/{name}.jpg";

            try
            {
                tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
                tex.Apply();


                File.WriteAllBytes(path, tex.EncodeToJPG(80));
                Debug.Log($"Picture saved to {path}");
            } finally {
                foreach (var c in allCanvas)
                    c.enabled = true;
            }

            ImageFlash.gameObject.SetActive(true);
            ImageFlash.color = Color.white;
            this.m_AudioSource.Play();

            yield return new WaitForSeconds(0.1f);
            while (ImageFlash.color.a > 0)
            {
                yield return new WaitForEndOfFrame();
                ImageFlash.color = new Color(1, 1, 1, ImageFlash.color.a - Time.deltaTime);
            }

            this.m_SnapCoroutine = null;
            this.Outcome = name;
        }
    }

}