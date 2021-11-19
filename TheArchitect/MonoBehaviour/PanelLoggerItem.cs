using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PanelLoggerItem : MonoBehaviour
{
    [SerializeField] public Text Text;
    [SerializeField] public Image Image;
    public UnityEvent OnLeft = new UnityEvent();

    public void Leave()
    {
        this.GetComponent<Animator>().SetTrigger("leave");
    }

    public void MarkLeft()
    {
        this.OnLeft.Invoke();
    }
}
