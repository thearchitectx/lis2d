using System;
using UnityEngine;
using UnityEngine.UI;
using TheArchitect.Core;

namespace TheArchitect.MonoBehaviour
{
	public class PanelTrophies : UnityEngine.MonoBehaviour 
	{
		[SerializeField] private RectTransform m_PanelContent;
		[SerializeField] private GameObject m_PrefabItem;
		[SerializeField] private Button m_ButtonClear;
		[SerializeField] private Button m_ButtonBack;

		public Button ButtonBack { get { return this.m_ButtonBack; } }

		void Start()
		{
			const string CONFIRM_TEXT = "CONFIRM?";
			
			this.m_ButtonClear.onClick.AddListener( () => {
				Text text = this.m_ButtonClear.GetComponentInChildren<Text>();
				if (text.text != CONFIRM_TEXT)
					text.text = CONFIRM_TEXT;
				else
				{
					text.text = "CLEARED";
					Trophies.WipeUnlockData();
					Build();
				}
			});

			Build();
		}

		void Build() 
		{
			foreach (Transform item in this.m_PanelContent) {
				Destroy(item.gameObject);
			}

			foreach (var trophy in Trophies.GetTrophies())
			{
				PanelTrophyItem panelTrophyItem = GameObject.Instantiate(this.m_PrefabItem).GetComponent<PanelTrophyItem>();
				panelTrophyItem.transform.SetParent(m_PanelContent, false);
				panelTrophyItem.SetTrophy(trophy);
			} 
		}

	}
}

