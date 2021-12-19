using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HideWhenConterNotOnCamera : MonoBehaviour
{
    [SerializeField] private float m_OpacityWhenVisible = 1;
    [SerializeField] private float m_FadeSpeed = 1;
    [SerializeField] private float m_BorderPadding = 0;
    private SpriteRenderer m_Renderer ;

    void Start()
    {
        this.m_Renderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 v = Camera.main.WorldToViewportPoint(this.transform.position);
        float target =  ( v.x > 0 - m_BorderPadding && v.x < 1 + m_BorderPadding && v.y > 0 - m_BorderPadding && v.y < 1 + m_BorderPadding)
            ? this.m_OpacityWhenVisible : 0;

        Color c = this.m_Renderer.color;
        c.a = Mathf.MoveTowards(c.a, target,  this.m_FadeSpeed * Time.deltaTime); 

        this.m_Renderer.color = c;
    }
}
