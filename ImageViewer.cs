using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ImageViewer : MonoBehaviour
{
    [SerializeField] private Image m_Image;
    [SerializeField] private float m_MouseSpeed = 1;
    [SerializeField] private float m_MouseScrollMultiplier = 1;

    private RectTransform m_ParentRect;
    private float m_TargetZoom = 1;
    private float m_Zoom = 1;
    private float m_FadeDestroyTime = 0;

    public void SetImage(string spriteKey)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(spriteKey);
        handle.Completed += (h) => {
            this.m_Image.sprite = h.Result;
        };
    }

    public void FadeAndDestroy()
    {
        this.m_FadeDestroyTime = Time.time + 0.5f;
        Destroy(this.gameObject, 0.5f);
    }

    void OnDestroy()
    {
        Addressables.Release(this.m_Image.sprite);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.m_ParentRect = this.m_Image.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        this.m_Image.color = new Color(1,1,1,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.m_Image.sprite == null)
            return;

        Vector2 v = new Vector3(Input.GetAxis("Mouse X") * m_MouseSpeed, Input.GetAxis("Mouse Y") * m_MouseSpeed, 0);
        this.m_TargetZoom = Mathf.Clamp(this.m_TargetZoom + (Input.mouseScrollDelta.y * this.m_MouseScrollMultiplier), 1, 2);
        this.m_Zoom = Mathf.MoveTowards(this.m_Zoom, this.m_TargetZoom, Time.deltaTime * 2f);

        this.m_Image.color = new Color(1,1,1,this.m_Image.color.a + Time.deltaTime * 2);

        float ratio = this.m_Image.sprite.texture.width / this.m_Image.sprite.texture.height;
        float ratioParent = m_ParentRect.rect.width / m_ParentRect.rect.height;

        this.m_Image.rectTransform.sizeDelta = new Vector2(
            ratio >= 1 ? m_ParentRect.rect.width : m_ParentRect.rect.width * ratioParent,
            ratio < 1 ? m_ParentRect.rect.height : m_ParentRect.rect.height * ratioParent
        );
        this.m_Image.rectTransform.sizeDelta *= this.m_Zoom;

        v += this.m_Image.rectTransform.anchoredPosition;
        
        var xLimit = (this.m_Image.rectTransform.sizeDelta.x - m_ParentRect.rect.width) / 2;
        var yLimit = (this.m_Image.rectTransform.sizeDelta.y - m_ParentRect.rect.height) / 2;
        v.x = Mathf.Clamp(v.x, -xLimit, xLimit);
        v.y = Mathf.Clamp(v.y, -yLimit, yLimit );

        this.m_Image.rectTransform.anchoredPosition = v;

        if (this.m_FadeDestroyTime > 0)
        {
            this.m_Image.color = new Color(1,1,1, this.m_Image.color.a - Time.time * 2 );
        }
    }
}
