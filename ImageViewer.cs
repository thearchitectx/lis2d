using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ImageViewer : MonoBehaviour
{
    [SerializeField] private Image m_Image;
    [SerializeField] private float m_MouseSpeed = 1;
    [SerializeField] private float m_MouseScrollMultiplier = 1;
    [SerializeField] private bool m_ReadInput = true;

    private RectTransform m_ParentRect;
    private float m_TargetZoom = 1;
    private float m_Zoom = 1;

    public void SetReadInput(bool b)
    {
        this.m_ReadInput = b;
    }

    public void SetImage(string spriteKey)
    {
        if (this.m_Image.sprite != null)
            Addressables.Release(this.m_Image.sprite);

        var handle = Addressables.LoadAssetAsync<Sprite>(spriteKey);
        handle.Completed += (h) => {
            this.m_Image.sprite = h.Result;
            Debug.Log($"Loaded {spriteKey} to {h.Result} with texture  {h.Result.texture}");
        };
    }

    public void SetImageScale(float scale)
    {
        this.m_Image.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void FadeAndDestroy()
    {
        StartCoroutine(FadeAndDestroyCoroutine());
    }

    private System.Collections.IEnumerator FadeAndDestroyCoroutine()
    {
        float a = Mathf.Clamp01(this.m_Image.color.a);
        while (a > 0)
        {
            this.m_Image.color = new Color(1, 1, 1, a);
            a -= Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }

        if (!Addressables.ReleaseInstance(this.gameObject))
            Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        Addressables.Release(this.m_Image.sprite);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.m_ParentRect = this.m_Image.canvas.GetComponent<RectTransform>();
        this.m_Image.color = new Color(1,1,1,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.m_Image == null || this.m_Image.sprite == null || this.m_Image.sprite.texture == null)
            return;

        Vector2 v = Vector2.zero;
        if (this.m_ReadInput)
        {
            v = new Vector3(Input.GetAxis("Mouse X") * m_MouseSpeed, Input.GetAxis("Mouse Y") * m_MouseSpeed, 0);
            this.m_TargetZoom = Mathf.Clamp(this.m_TargetZoom + (Input.mouseScrollDelta.y * this.m_MouseScrollMultiplier), 1, 2);
        }

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
    }
}
