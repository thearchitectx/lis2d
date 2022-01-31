using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAxisTranslateController : MonoBehaviour
{
    [SerializeField] private float m_Speed = 1;
    [SerializeField] private Collider2D m_Confiner;
    [SerializeField] private Vector3 m_RecenterPosition;
    [SerializeField] private float m_RecenterSpeed = 10;

    private bool m_RecenterPending;
    private bool m_ReadInput = true;

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime == 0)
            return;

        if (this.m_RecenterPending)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.m_RecenterPosition, Time.deltaTime * m_RecenterSpeed );
            this.m_RecenterPending = this.transform.position != this.m_RecenterPosition;
            return;
        }

        if (!this.m_ReadInput || Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(1))
            return;

        Vector3 v = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0 );
        this.transform.position += v * this.m_Speed;

        if (this.m_Confiner!=null)
        {
            if (!this.m_Confiner.OverlapPoint(this.transform.position))
            {
                this.transform.position = this.m_Confiner.ClosestPoint(this.transform.position);
            }
        }

    }

    [ContextMenu("Recenter Position")]
    public void Recenter() {
        this.m_RecenterPending = true;
    }

    public void SetReadInput(bool read) {
        this.m_ReadInput = read;
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        float p = 0.5f;
        Gizmos.DrawLine(this.transform.position + new Vector3(-p,-p,0), this.transform.position + new Vector3(p,p,0));
        Gizmos.DrawLine(this.transform.position + new Vector3(p,-p,0), this.transform.position + new Vector3(-p,p,0));
    }
}
