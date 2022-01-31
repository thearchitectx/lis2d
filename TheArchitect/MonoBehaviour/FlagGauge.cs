using System;
using UnityEngine;
using UnityEngine.UI;
using TheArchitect.Game;
using TheArchitect.SceneObjects;

public class FlagGauge : SceneObject
{
    [SerializeField] private AssetReferenceGameContext m_GameContext;
    [SerializeField] private Image m_ImageBG;
    [SerializeField] private Image m_ImageValue;
    [SerializeField] private Text m_TextLabel;
    [SerializeField] private int m_MinValue = 0;
    [SerializeField] private int m_MaxValue = 100;
    [SerializeField] private string m_Flag = "FLAG";
    [SerializeField] private Color32 m_MinColor = new Color(0xFF, 0xEC, 0x00);
    [SerializeField] private Color32 m_MaxColor = new Color(0xFF, 0x00, 0xA1);

    private GameContext m_Context;
    private Animator m_Animator;
    private int m_LastStageChangeCount = int.MinValue;
    private float m_CurrentValue;

    public void SetFlag(string flag)
    {
        this.m_Flag = flag;
    }

    public void SetLabel(string label)
    {
        this.m_TextLabel.text = label;
    }

    public void SetMinValue(int min)
    {
        this.m_MinValue = min;
    }

    public void SetMaxValue(int max)
    {
        this.m_MaxValue = max;
    }

    public void SetMinColor(string color)
    {
        Color c;
        if ( ColorUtility.TryParseHtmlString(color, out c) )
            this.m_MinColor = c;
    }

    public void SetMaxColor(string color)
    {
        Color c;
        if ( ColorUtility.TryParseHtmlString(color, out c) )
            this.m_MaxColor = c;
    }

    void Start()
    {
        this.m_GameContext.LoadAssetAsync().Completed += handle => {
            this.m_Context = handle.Result;
        };
        this.m_Animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (this.m_Context == null)
            return;

        if (this.m_LastStageChangeCount != this.m_Context.StateChangeCount)
        {
            this.m_LastStageChangeCount = this.m_Context.StateChangeCount;
            this.m_CurrentValue = Mathf.InverseLerp(this.m_MinValue, this.m_MaxValue, this.m_Context.GetVariable(m_Flag, 0));
        }

        this.m_ImageValue.fillAmount = Mathf.MoveTowards(this.m_ImageValue.fillAmount, this.m_CurrentValue, Time.deltaTime * 0.25f);
        this.m_ImageValue.color = Color32.Lerp(this.m_MinColor, this.m_MaxColor, this.m_ImageValue.fillAmount);

        if (this.m_Animator!=null)
        {
            this.m_Animator.SetBool("full", this.m_ImageValue.fillAmount >= 1);
        }
    }
}