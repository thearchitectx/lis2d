using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public void SelfDestroyNow()
    {
        if (!UnityEngine.AddressableAssets.Addressables.ReleaseInstance(this.gameObject))
            Destroy(this.gameObject);
    }

}
