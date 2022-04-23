
using UnityEngine;

public class SpringBone : MonoBehaviour
{
    private SpringBone child;

    public float radius = 0.5f;

    public float stiffnessForce = 0.2f;

    public float dragForce = 0.1f;

    public Vector3 springForce = new Vector3(0.0f, -0.05f, 0.0f);

    public SpringCollider[] colliders;

    public bool debug;
    public bool RecalculateChildSpringLength = true;
    

    public float springLength;
    private Quaternion localRotation;
    private Vector3 currTipPos;
    private Vector3 prevTipPos;
    private Vector3 startTipPos;

    private void Awake()
    {
        localRotation = transform.localRotation;
    }

    public SpringBone Child 
    {
        get
        {
            var springBones=GetComponentsInChildren<SpringBone>();
            if(springBones.Length>1)
            {
                child= springBones[1];
            }
            return child;

        }

        set
        {
            child=value;
        }
    }

    private void OnEnable()
    {
        if (Child && RecalculateChildSpringLength)
        {
            springLength = Vector3.Distance(transform.position, Child.transform.position);
            currTipPos = Child.transform.position;
            prevTipPos = Child.transform.position;
        }
        else
        {
            startTipPos = currTipPos = prevTipPos = transform.position + (transform.right) * springLength;
        }

    }

    public void UpdateSpring()
    {
        if (!this.enabled)
            return;
            
        if (Time.timeScale > 1)
        {
            currTipPos = prevTipPos = startTipPos;
            return;
        }
            
        transform.localRotation = Quaternion.identity * localRotation;

        float sqrDt = Time.deltaTime * Time.deltaTime;

        Vector3 force = transform.rotation * (Vector3.right * stiffnessForce) / sqrDt;

        force += (prevTipPos - currTipPos) * dragForce / sqrDt;

        force += springForce / sqrDt;

        Vector3 temp = currTipPos;

        currTipPos = (currTipPos - prevTipPos) + currTipPos + (force * sqrDt);

        currTipPos = ((currTipPos - transform.position).normalized * springLength) + transform.position;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (Vector3.Distance(currTipPos, colliders[i].transform.position) <= (radius + colliders[i].radius))
            {
                Vector3 normal = (currTipPos - colliders[i].transform.position).normalized;
                currTipPos = colliders[i].transform.position + (normal * (radius + colliders[i].radius));
                currTipPos = ((currTipPos - transform.position).normalized * springLength) + transform.position;
            }
        }

        prevTipPos = temp;

        Vector3 aimVector = transform.TransformDirection(Vector3.right);
        Quaternion aimRotation = Quaternion.FromToRotation(aimVector, currTipPos - transform.position);
        transform.rotation = aimRotation * transform.rotation;

    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(currTipPos, radius);
        }
    }

}