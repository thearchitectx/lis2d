using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PanelLogger : MonoBehaviour
{
    [SerializeField] private PanelLoggerItem m_ItemPrefab;
    [SerializeField] private Transform m_PanelRoot;

    private float m_TimeToClose;

    public void AddItem(string text, string icon)
    {
        this.m_TimeToClose = 0;

        var i = GameObject.Instantiate(this.m_ItemPrefab.gameObject).GetComponent<PanelLoggerItem>();
        i.Text.text = text;
        if (!string.IsNullOrEmpty(icon))
        {
            Addressables.LoadAssetAsync<Sprite>(icon).Completed += (handle) => {
                i.Image.sprite = handle.Result;
            };
        }
        i.Image.gameObject.SetActive(icon != null);
        i.transform.SetParent(this.m_PanelRoot, false);
        
        i.OnLeft.AddListener( () => {
            i.transform.SetParent(null);

            if (i.Image.sprite!=null)
                Addressables.Release(i.Image.sprite);
                
            if (!Addressables.ReleaseInstance(i.gameObject))
                Destroy(i.gameObject);

            var next = this.m_PanelRoot.GetComponentInChildren<PanelLoggerItem>();

            if (next != null)
            {
                next.Leave();
            }
            else
            {
                this.m_TimeToClose = Time.time + 3;
            }
        });

        if (this.m_PanelRoot.childCount == 1)
            i.Leave();

    }


}
