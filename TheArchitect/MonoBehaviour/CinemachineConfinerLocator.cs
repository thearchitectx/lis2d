using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof (CinemachineConfiner))]
public class CinemachineConfinerLocator : MonoBehaviour
{
    [SerializeField] private string m_ConfinerTag = "CameraConfiner";
    void Start()
    {
        var g = GameObject.FindWithTag(this.m_ConfinerTag);
        if (g != null) {
            this.GetComponent<CinemachineConfiner>().m_BoundingShape2D  = g.GetComponent<Collider2D>();
        }
    }
}
