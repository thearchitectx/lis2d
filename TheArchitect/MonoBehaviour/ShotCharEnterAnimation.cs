using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ShotCharEnterAnimation : MonoBehaviour
{
    [SerializeField] private Transform m_ShotCharRoot;
    [SerializeField] private float m_Speed = 1;
    [SerializeField] private float m_StartX = -15;
    [SerializeField] private float m_EndX = 0;

    private Parallax m_Parallax;

    private bool m_KeepFinalPosition = false;
    private float m_Progress = 0;
    private bool m_DoAnimate = false;

    void Start()
    {
        this.m_Parallax = this.GetComponent<Parallax>();
    }

    public void TriggerEnter()
    {
        this.m_KeepFinalPosition = false;
        this.m_DoAnimate = true;
        this.m_Progress = 0;
        UpdatePosition();
    }

    public void TriggerEnterAndKeep()
    {
        this.m_KeepFinalPosition = true;
        this.m_DoAnimate = true;
        this.m_Progress = 0;
        UpdatePosition();
    }

    public ShotCharEnterAnimation SetChar(Transform charRoot)
    {
        this.m_ShotCharRoot = charRoot;
        return this;
    }

    public ShotCharEnterAnimation StartCurrent()
    {
        this.m_StartX = this.m_ShotCharRoot != null ? this.m_ShotCharRoot.position.x : this.m_StartX;
        return this;
    }

    public ShotCharEnterAnimation StartCenter()
    {
        this.m_StartX = 0;
        return this;
    }

    public ShotCharEnterAnimation StartLeft()
    {
        this.m_StartX = -15;
        return this;
    }

    public ShotCharEnterAnimation StartRight()
    {
        this.m_StartX = 15;
        return this;
    }

    public ShotCharEnterAnimation EndFarLeft()
    {
        this.m_EndX = -30;
        return this;
    }

    public ShotCharEnterAnimation EndFarRight()
    {
        this.m_EndX = 30;
        return this;
    }

    public ShotCharEnterAnimation EndRight()
    {
        this.m_EndX = 15;
        return this;
    }

    public ShotCharEnterAnimation EndSideRight()
    {
        this.m_EndX = 4.75f;
        return this;
    }

    public ShotCharEnterAnimation StartSideRight()
    {
        this.m_StartX = 4.75f;
        return this;
    }

    public ShotCharEnterAnimation StartSideLeft()
    {
        this.m_StartX = -4.75f;
        return this;
    }

    public ShotCharEnterAnimation EndSideLeft()
    {
        this.m_EndX = -4.75f;
        return this;
    }

    public ShotCharEnterAnimation EndLeft()
    {
        this.m_EndX = -15;
        return this;
    }

    public ShotCharEnterAnimation EndCenter()
    {
        this.m_EndX = 0;
        return this;
    }

    public void SetSpeed(float speed)
    {
        this.m_Speed = Mathf.Clamp(speed, .1f, 10);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (this.m_ShotCharRoot == null || !this.m_DoAnimate)
            return;

        UpdatePosition();


        if (this.m_Progress >= 1) {
            this.m_DoAnimate = this.m_KeepFinalPosition;
            this.m_Progress = this.m_KeepFinalPosition ? 1 : 0;
        }
        else
            this.m_Progress += Time.deltaTime * this.m_Speed;
    }

    private void UpdatePosition()
    {
        float x = Mathf.SmoothStep(this.m_StartX, this.m_EndX, this.m_Progress);

        this.m_ShotCharRoot.transform.position = new Vector3(
            x,
            this.m_ShotCharRoot.transform.position.y,
            this.m_ShotCharRoot.transform.position.z
        );

        if (this.m_Parallax != null)
        {
            this.m_Parallax.ReferencePosition = new Vector2(x, this.m_Parallax.ReferencePosition.y);
        }
    }
}
