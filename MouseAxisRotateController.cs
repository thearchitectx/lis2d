using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAxisRotateController : MonoBehaviour
{
    [SerializeField] private float m_Speed = 1;
    [SerializeField] private float m_MinZ = -180;
    [SerializeField] private float m_MaxZ = +180;

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime == 0)
            return;

        Vector3 v = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0 );
        this.transform.Rotate(new Vector3(0,0, (v.x+v.y) * this.m_Speed));
        v = this.transform.rotation.eulerAngles;
        v.z = Mathf.Clamp(v.z, this.m_MinZ, this.m_MaxZ);
        this.transform.rotation = Quaternion.Euler(v);
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.transform.position, 0.25f);
    }
}
