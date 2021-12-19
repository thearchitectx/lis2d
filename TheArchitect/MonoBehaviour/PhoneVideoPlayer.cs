using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PhoneVideoPlayer : TheArchitect.SceneObjects.SceneObject
{
    public const string OUTCOME_CLOSE = "CLOSE";

    [SerializeField] private Sprite m_SpritePlay;
    [SerializeField] private Sprite m_SpritePause;
    [SerializeField] private Button m_ButtonPlay;
    [SerializeField] private Button m_ButtonSpeed;
    [SerializeField] private Button m_ButtonClose;
    [SerializeField] private Image m_ImageProgress;
    [SerializeField] private VideoPlayer m_Player;
    [SerializeField] private float m_HideTime = 3;
    
    private Canvas m_Canvas;
    private Image m_ButtonPlayImage;
    private Text m_ButtonSpeedText;
    private Animator m_Animator;
    private float m_MouseTimer;
    private bool m_ForceHide = false;

    // Start is called before the first frame update
    void Start()
    {
        this.m_Canvas = this.m_ImageProgress.GetComponentInParent<Canvas>();
        this.m_ButtonPlayImage = this.m_ButtonPlay.GetComponent<Image>();
        this.m_ButtonSpeedText = this.m_ButtonSpeed.GetComponentInChildren<Text>();
        this.m_Animator = this.GetComponent<Animator>();

        this.m_ButtonPlay.onClick.AddListener(() => {
            if ( this.m_Player.isPaused )
                this.m_Player.Play();
            else
                this.m_Player.Pause();
        });

        this.m_ButtonSpeed.onClick.AddListener( () => {
            double s = this.m_Player.playbackSpeed;
            this.m_Player.playbackSpeed = Mathf.Repeat((float) s - 0.25f, 1.75f);
        });

        this.m_ButtonClose.onClick.AddListener( () => Outcome = OUTCOME_CLOSE);

        this.m_MouseTimer = 0;

        this.m_Player.Play();
    }

    public void DisableUI()
    {
        this.m_ForceHide = true;
    }
    
    public void EnableUI()
    {
        this.m_ForceHide = false;
        this.m_MouseTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime==0)
            return;

        this.m_MouseTimer += Time.deltaTime;

        if (Input.GetAxis("Mouse X")!=0 || Input.GetAxis("Mouse Y")!=0 || Input.GetMouseButton(0))
            this.m_MouseTimer = 0;
        
        this.m_Animator.SetBool("on", this.m_MouseTimer < this.m_HideTime && !m_ForceHide);

        if (Input.GetMouseButton(0))
        {
            Rect area = new Rect(m_ImageProgress.rectTransform.rect.x,
                m_ImageProgress.rectTransform.rect.y,
                m_ImageProgress.rectTransform.rect.width,
                50);
            Vector2 vec;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    m_ImageProgress.rectTransform,
                    Input.mousePosition,
                    this.m_Canvas.worldCamera,
                    out vec
            );
            if (area.Contains(vec) )
            {
                vec = Rect.PointToNormalized(m_ImageProgress.rectTransform.rect, vec);
                this.m_Player.Pause();
                this.m_Player.time = ( vec.x * this.m_Player.clip.length );
            }
        }

        m_ImageProgress.fillAmount = (float) ( this.m_Player.time / this.m_Player.clip.length );

        double speed = this.m_Player.playbackSpeed;
        this.m_ButtonSpeedText.text = $"{speed:0.##}x";
        this.m_ButtonSpeedText.color = speed != 1 ? Color.cyan : Color.white;
        this.m_ButtonPlayImage.sprite = this.m_Player.isPaused ? m_SpritePlay : m_SpritePause;
    }

}
