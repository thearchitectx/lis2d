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
        [SerializeField] private AssetReferenceGameContext m_Context;
        [SerializeField] private string m_Label;
        [SerializeField] private Transform m_OnNoticed;
        [SerializeField] private Transform m_OnActivated;
        [SerializeField] private Text m_Text;
        [SerializeField] private bool m_CrouchOnly;

        public Transform OnActivated { get { return this.m_OnActivated; } }
        public Transform OnNoticed { get { return this.m_OnNoticed; } }
        public string Label { get { return this.m_Label; } }
        public bool CrouchOnly { get { return this.m_CrouchOnly; } }

        void Start()
        {
            this.m_Context.LoadAssetAsync().Completed += (handle) => {
                this.m_Label = ResourceString.Parse(this.m_Label, handle.Result.GetVariable).ToUpper();
                if (this.m_Text != null)
                {
                    this.m_Text.text = this.m_Label;
                }
                m_Context.ReleaseAsset();
            };
        }

    }
}
