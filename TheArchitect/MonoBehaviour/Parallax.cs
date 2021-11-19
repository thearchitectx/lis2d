﻿using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Vector3 m_ReferenceCameraOrigin;
    [SerializeField] private Transform m_ReferenceBackground;
    [SerializeField] private float m_Speed = 1f;
    
    [SerializeField] private bool m_UseStartReferencePosition = true;
    [SerializeField] private Vector2 m_ReferencePosition;

    [SerializeField] private float m_RotateFactorY;
    
    void Start()
    {
        if (this.m_UseStartReferencePosition)
            this.m_ReferencePosition = this.transform.position;

        if (this.m_ReferenceBackground == null)
            this.m_ReferenceBackground = this.transform.parent;
    }

    public void FindReferenceBackground(string name)
    {
        this.m_ReferenceBackground = GameObject.Find(name)?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.m_ReferenceBackground == null)
            return;

        Vector2 cameraOffset = Camera.main.transform.position - this.m_ReferenceBackground.position;

        this.transform.position = this.m_ReferencePosition - ( cameraOffset * this.m_Speed );

        if (this.m_RotateFactorY != 0)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, cameraOffset.magnitude * Mathf.Sign(cameraOffset.y) * this.m_RotateFactorY);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(this.m_ReferenceCameraOrigin, Vector3.one * 0.25f);
        Gizmos.DrawLine(this.m_ReferenceCameraOrigin, Camera.main.transform.position);
    }

    [ContextMenu("Copy Current Main Camera Origin")]
    public void CopyReferenceCameraOrigin() {
        this.m_ReferenceCameraOrigin = Camera.main.transform.transform.position;
    }
}
