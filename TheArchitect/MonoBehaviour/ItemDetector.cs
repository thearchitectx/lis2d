using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TheArchitect.SceneObjects;

public class ItemDetector : SceneObject
{
    [SerializeField] public AudioSource BeepSource;
    [SerializeField] public Image ItemImage;
    [SerializeField] public string ItemTag;
    [SerializeField] public LayerMask DetectableLayer;
    [SerializeField] public float DetectRadius = 25;
    [SerializeField] public float MinImageSize = 20;
    [SerializeField] public float MaxImageSize = 100;
    [SerializeField] public float MinBeepPitch = 0.9f;
    [SerializeField] public float MaxBeepPitch = 1.5f;

    private float m_NextScanTime = 0;
    private List<Collider2D> m_Results = new List<Collider2D>();

    public void SetItemTag(string tag)
    {
        this.ItemTag = tag;
    }

    void Start()
    {
        BeepSource.gameObject.SetActive(false);
        ItemImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Time.deltaTime == 0)
        {
            BeepSource.gameObject.SetActive(false);
            ItemImage.gameObject.SetActive(false);
            return;
        }

        if (Time.time > this.m_NextScanTime)
        {
            int qtd = Physics2D.OverlapCircle(this.transform.position, DetectRadius, new ContactFilter2D() { layerMask=DetectableLayer, useTriggers=true, useLayerMask=true }, m_Results);
            
            bool detected = false;
            if (qtd > 0)
            {
                this.m_Results.ForEach( i => {
                    if (i!= null && i.gameObject.tag == ItemTag)
                    {
                        detected = true;
                        var diff = (i.transform.position - this.transform.position);
                        var normalizedDistance = Mathf.InverseLerp(0.5f, DetectRadius, diff.magnitude );
                        // Debug.Log($"{i.name}: {diff.magnitude} {normalizedDistance} {i.gameObject.tag}");

                        var size = Mathf.Lerp(MaxImageSize, MinImageSize, normalizedDistance);
                        ItemImage.rectTransform.sizeDelta = new Vector2(size, size);
                        BeepSource.pitch = Mathf.Lerp(MaxBeepPitch, MinBeepPitch, normalizedDistance);
                    }
                });
            }

            BeepSource.gameObject.SetActive(detected && Time.deltaTime > 0);
            ItemImage.gameObject.SetActive(detected && Time.deltaTime > 0);

            this.m_NextScanTime = Time.time + (detected ? 0.5f : 2f);
        }
        
    }

}

