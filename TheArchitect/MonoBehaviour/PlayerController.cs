using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheArchitect.Core;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator m_SkeletonRoot;
    [SerializeField] private Animator m_SkeletonRootMirror;
    [SerializeField] private Transform m_PerceptionField;
    [SerializeField] private float m_Speed;
    [SerializeField] private bool m_EnableCrouch = true;

    private bool m_InputActive = true;
    private bool m_Crouching = false;
    private Rigidbody2D m_RigidBody;

    public bool Crouching { get { return this.m_Crouching; }}

    public void LookAt(Quaternion rot)
    {
        if (rot.eulerAngles.y > 0)
            LookLeft();
        else
            LookRight();
    }

    public void SetInputActive(bool active)
    {
        this.m_InputActive = active;
    }

    public float GetLookingDir()
    {
        return ((Vector2)this.m_PerceptionField.right) == Vector2.left ? 0 : 180;
    }

    public void LookLeft()
    {
        this.m_SkeletonRootMirror.gameObject.SetActive(true);
        this.m_SkeletonRoot.gameObject.SetActive(false);
        this.m_PerceptionField.right = Vector2.left;
    }

    public void WarpTo(string objectName)
    {
        var g = GameObject.Find(objectName);
        if (g != null)
        {
            this.transform.position = g.transform.position;
            this.LookAt(g.transform.rotation);
        }
    }

    public void LookRight()
    {
        this.m_SkeletonRootMirror.gameObject.SetActive(false);
        this.m_SkeletonRoot.gameObject.SetActive(true);
        this.m_PerceptionField.right = Vector2.right;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.m_RigidBody = this.GetComponent<Rigidbody2D>();
        if (this.m_SkeletonRootMirror.gameObject.activeSelf && this.m_SkeletonRoot.gameObject.activeSelf)
            LookRight();
    }
    

    // Update is called once per frame
    void Update()
    {
        var x = this.m_InputActive ? Input.GetAxisRaw("Horizontal") : 0; 
        var y = this.m_InputActive ? Input.GetAxisRaw("Vertical") : 0; 

        if (this.m_InputActive && this.m_EnableCrouch)
            this.m_Crouching = y < 0;
            
        this.m_RigidBody.velocity = new Vector2( y < 0 ? 0 :  x * this.m_Speed, this.m_RigidBody.velocity.y);
        
        if (this.m_RigidBody.velocity.x > 0)
            LookRight();
        else  if (this.m_RigidBody.velocity.x < 0)
            LookLeft();

        if (this.m_SkeletonRoot.gameObject.activeInHierarchy)
        {
            this.m_SkeletonRoot.SetBool("walking", this.m_RigidBody.velocity.x > 0);
            this.m_SkeletonRoot.SetBool("crouch", this.m_Crouching);
        }
        else if (this.m_SkeletonRootMirror.gameObject.activeInHierarchy)
        {
            this.m_SkeletonRootMirror.SetBool("walking", this.m_RigidBody.velocity.x < 0);
            this.m_SkeletonRootMirror.SetBool("crouch", this.m_Crouching);
        }
    }
}
