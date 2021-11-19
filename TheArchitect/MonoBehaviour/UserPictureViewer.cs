using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TheArchitect.Game;

namespace TheArchitect.SceneObjects
{

    public class UserPictureViewer : SceneObject
    {
        [SerializeField] private RectTransform m_Container;
        [SerializeField] private Image m_ImagePicture;
        [SerializeField] private Image m_ImageForward;

        private float m_Timer = 0.5f;
        private int m_FixedWidth = int.MinValue;

        public void SetCanvasSortOrder(int order)
        {
            m_Container.GetComponentInParent<Canvas>().sortingOrder = order;
        }

        public void SetFixedWidth(int width)
        {
            this.m_FixedWidth = width;
        }

        public void FixedScreenWidth()
        {
            this.m_FixedWidth = 500;
        }

        public void LoadPicture(string name)
        {
            var path = $"{Application.persistentDataPath}/{GameStateIO.PICS_FOLDER}/{name}.jpg";
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.DoWork += (sender, args) => 
                args.Result = File.Exists(path)
                    ? File.ReadAllBytes(path)
                    : new byte[0];

            worker.RunWorkerCompleted += (sender, args) => {
                var t = new Texture2D(256, 256, TextureFormat.RGB24, false);
                t.LoadImage((byte[]) args.Result);
                this.m_ImagePicture.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(t.width/2, t.height/2), 100);

                if (this.m_FixedWidth > 0)
                    this.m_Container.sizeDelta = new Vector2(this.m_FixedWidth, this.m_FixedWidth * t.height / t.width);
                else
                    this.m_Container.sizeDelta = new Vector2(t.width, t.height);
            };
            worker.RunWorkerAsync();
        }

        public void SetImageForwardVisible(bool visible)
        {
            this.m_ImageForward.gameObject.SetActive(visible);
        }

        public void Leave()
        {
            this.GetComponent<Animator>().SetTrigger("leave");
        }

        public void Destroy()
        {
            if (!UnityEngine.AddressableAssets.Addressables.ReleaseInstance(this.gameObject))
                Destroy(this.gameObject);
        }

        void Update()
        {
            if (this.m_Timer <= 0 && (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Jump")))
            {
                this.Outcome = "DONE";
            }

            this.m_Timer -= Time.deltaTime;
        }
    }

}

