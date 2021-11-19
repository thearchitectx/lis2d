using UnityEngine;
using UnityEngine.UI;
using TheArchitect.SceneObjects;

public class Interrupt : SceneObject
{
    public const string OUTCOME_TIME_UP = "TIME_UP";
    public const string OUTCOME_PARAGON = "PARAGON";
    public const string OUTCOME_RENEGADE = "RENEGADE";
    [SerializeField] private Image m_ImageFillRenegade;
    [SerializeField] private Image m_ImageFillParagon;

    private float m_StartProccessTime;
    private float m_EndProccessTime;

    public void SetWaitInputTime(float time)
    {
        this.m_EndProccessTime = Time.time + time;
    }

    public void EnableParagon()
    {
        this.m_ImageFillParagon.gameObject.SetActive(true);
    }

    public void EnableRenegade()
    {
        this.m_ImageFillRenegade.gameObject.SetActive(true);
    }

    void Start()
    {
        this.m_StartProccessTime = Time.time;
        if (this.m_EndProccessTime==0)
            this.m_EndProccessTime = Time.time + 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < this.m_StartProccessTime + 0.25f || !string.IsNullOrEmpty(this.Outcome) )
            return;

        if (Time.time > this.m_EndProccessTime)
        {
            this.Outcome = OUTCOME_TIME_UP;
            return;
        }

        this.m_ImageFillRenegade.fillAmount = this.m_ImageFillParagon.fillAmount = Mathf.InverseLerp(this.m_EndProccessTime, this.m_StartProccessTime, Time.time);

        if (Input.GetMouseButtonDown(0) && this.m_ImageFillParagon.gameObject.activeSelf)
        {
            this.Outcome = OUTCOME_PARAGON;
            return;
        }

        if (Input.GetMouseButtonDown(1) && this.m_ImageFillRenegade.gameObject.activeSelf)
        {
            this.Outcome = OUTCOME_RENEGADE;
            return;
        }
        
    }
}
