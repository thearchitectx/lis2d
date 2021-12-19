using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheArchitect.MonoBehaviour.Save
{

    public class PanelSaveItem : UnityEngine.MonoBehaviour
    {
        public Image ImageItemBG;
        public Image ImageScreen;
        public Text TextSlot;
        public Text TextLabel;
        public Text TextDate;
        public InputField InputLabel;
        public Button ButtonLoad;
        public Button ButtonSwitch;
        public Button ButtonRename;
        public Button ButtonDelete;

        public bool IsCurrentItem = false;

        void Update()
        {
            if (IsCurrentItem)
                this.ImageItemBG.color = new Color(0, Mathf.PingPong(Time.unscaledTime, 0.5f) + 0.1f, Mathf.PingPong(Time.unscaledTime, 0.5f) + 0.1f, 1);
        }
    }

}