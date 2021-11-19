using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheArchitect.Core;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator m_SkeletonRoot;
    [SerializeField] private float m_Speed;

    private bool m_InputActive = true;
    private bool m_Crouching = false;
    private Rigidbody2D m_RigidBody;

    public bool Crouching { get { return this.m_Crouching; }}

    public void SetInputActive(bool active)
    {
        this.m_InputActive = active;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.m_RigidBody = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var x = this.m_InputActive ? Input.GetAxisRaw("Horizontal") : 0; 
        var y = this.m_InputActive ? Input.GetAxisRaw("Vertical") : 0; 

        this.m_Crouching = y < 0;
        this.m_SkeletonRoot.SetBool("crouch", this.m_Crouching);

        this.m_RigidBody.velocity = new Vector2( y < 0 ? 0 :  x * this.m_Speed, this.m_RigidBody.velocity.y);
        
        if (this.m_RigidBody.velocity.x != 0)
        {
            this.m_SkeletonRoot.transform.right = new Vector2(this.m_RigidBody.velocity.x, 0);
        }

        this.m_SkeletonRoot.SetBool("walking", this.m_RigidBody.velocity.x != 0);
    }
}
