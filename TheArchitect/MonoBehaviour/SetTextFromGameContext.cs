using UnityEngine;
using TheArchitect.Core;
using TheArchitect.Game;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class SetTextFromGameContext : MonoBehaviour
{
    [SerializeField] private string m_Expression;
    [SerializeField] private bool m_UpperCase = true;
    [SerializeField] private AssetReferenceGameContext m_Context;

    // Start is called before the first frame update
    void Start()
    {
        this.m_Context.LoadAssetAsync().Completed += handle => {
            var t = ResourceString.Parse(this.m_Expression, handle.Result.GetVariable);
            GetComponent<UnityEngine.UI.Text>().text = this.m_UpperCase ? t.ToUpper() : t;
            UnityEngine.AddressableAssets.Addressables.Release(handle);
        };
    }

}
