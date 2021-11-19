using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PrefabAssetProxy : MonoBehaviour
{
    [SerializeField] private AssetReference m_AssetReference;

    void Start()
    {
        if (this.m_AssetReference != null)
        {
            this.m_AssetReference.InstantiateAsync(this.transform.parent).Completed += (handle) => {
                Destroy(this.gameObject);
            };
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

}
