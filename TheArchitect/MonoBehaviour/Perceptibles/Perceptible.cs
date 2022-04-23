using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using TheArchitect.Game;
using TheArchitect.Core;

namespace TheArchitect.MonoBehaviour.Perceptibles
{
    [RequireComponent(typeof(Collider2D))]
    public class Perceptible : UnityEngine.MonoBehaviour
    {
        public const string ITEM_GLINT_ACTIVATOR = "ITEM:PERCEPTION_BAND";
        [SerializeField] private AssetReferenceGameContext m_Context;
        [SerializeField] private string m_Label;
        [SerializeField] private Transform m_OnNoticed;
        [SerializeField] private Transform m_OnActivated;
        [SerializeField] private Transform m_Glint;
        [SerializeField] private Text m_Text;
        [SerializeField] private bool m_CrouchOnly;
        [SerializeField] private bool m_AutoActivate = false;

        public Transform OnActivated { get { return this.m_OnActivated; } }
        public Transform OnNoticed { get { return this.m_OnNoticed; } }
        public string Label { get { return this.m_Label; } }
        public bool CrouchOnly { get { return this.m_CrouchOnly; } }
        public bool AutoActivate { get { return this.m_AutoActivate; } }

        void Start()
        {
            this.m_Context.LoadAssetAsync().Completed += (handle) => {
                this.m_Label = ResourceString.Parse(this.m_Label, handle.Result.GetVariable).ToUpper();
                if (this.m_Text != null)
                {
                    this.m_Text.text = this.m_Label;
                }

                if (this.m_Glint == null)
                {
                    this.m_Glint = transform.Find("Glint");
                }
                if (this.m_Glint != null)
                {
                    this.m_Glint.gameObject.SetActive(this.m_CrouchOnly && handle.Result.GetVariable(ITEM_GLINT_ACTIVATOR, 0) > 0);
                }
                
                m_Context.ReleaseAsset();
            };
        }

    }
}
