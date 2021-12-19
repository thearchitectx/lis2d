using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TheArchitect.MonoBehaviour
{
    public class PointerDownButton : UnityEngine.MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private UnityEvent<PointerEventData> m_OnPointerDown;

        public void OnPointerDown(PointerEventData data)
        {
            this.m_OnPointerDown.Invoke(data);
        }
    }

}
