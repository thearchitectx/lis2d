using System;
using UnityEngine;
using TheArchitect.SceneObjects;
using UnityEngine.AddressableAssets;

namespace TheArchitect.MonoBehaviour
{
    struct TumblerControl
    {
        public Transform Root;
        public Transform Handle;
        public Transform Tumbler;
        public SpriteRenderer HandleRenderer;
        public float TargetY;
        public Vector3 Velocity;
        public float MaxSpeed;
    }

    public class Lockpick : SceneObject
    {
        const float TUMBLER_DIMENSION = 1.1f;
        const float TUMBLER_REST_Y  = -0.9463776f;
        public const string OUTCOME_UNLOCKED = "UNLOCKED";
        public const string OUTCOME_QUIT = "QUIT";
        [SerializeField] private int m_TumblerCount;
        [SerializeField] private Transform m_FrontPiece;
        [SerializeField] private Collider2D m_PickCollider;
        [SerializeField] private GameObject m_TumblerPrefab;
        [SerializeField] private float m_PickSpeed = 0.01f;
        [SerializeField] private LayerMask m_HandleLayerMask;

        [SerializeField] private AssetReference m_ClipUnlockRef;
        [SerializeField] private AssetReference m_ClipHitRef;
        [SerializeField] private AssetReference m_ClipFailRef;

        private AudioClip m_ClipUnlock;
        private AudioClip m_ClipHit;
        private AudioClip m_ClipFail;
        
        [SerializeField] private int[] m_TumblerSequence;
        [SerializeField] private TumblerControl[] m_Tumblers;
        
        private int m_SequencePosition = -1;
        private bool m_InputEnabled = true;

        public void DisableInput() { m_InputEnabled = false;}
        public void EnableInput() { m_InputEnabled = true; }

        public void Generate(int size)
        {
            this.m_TumblerCount = size;
            this.m_TumblerSequence = new int[size];
            this.m_TumblerSequence.Fill(-1);
            this.m_TumblerSequence[0] = size/2;

            for (int i=1; i<size; i++)
            {
                int p = this.m_TumblerSequence[i-1];
                int dir = UnityEngine.Random.value > 0.5f ? -1 : 1;
                int x = dir;
                while (Array.IndexOf( this.m_TumblerSequence, p+x) >= 0 && p+x>=0 && p+x < size  ) 
                    x += dir;

                if (p+x<0 || p+x>=size)
                {
                    dir = -dir;
                    x = dir;
                    while (Array.IndexOf( this.m_TumblerSequence, p+x) >= 0 && p+x>=0 && p+x < size  ) 
                        x += dir;
                }

                this.m_TumblerSequence[i] = p+x;
            }
            
            this.m_FrontPiece.transform.position += new Vector3(-TUMBLER_DIMENSION * (m_TumblerCount-1), 0, 0);
            this.m_Tumblers = new TumblerControl[m_TumblerCount];

            for (int i=0; i<this.m_TumblerCount; i++)
            {
                GameObject tumbler = Instantiate(this.m_TumblerPrefab);
                tumbler.transform.SetParent(this.transform, false);
                tumbler.gameObject.name = $"{i}";
                tumbler.transform.localPosition += new Vector3(-TUMBLER_DIMENSION * i, 0, 0);

                this.m_Tumblers[i] = new TumblerControl() {
                    Root = tumbler.transform,
                    Handle = tumbler.transform.Find("handle"),
                    HandleRenderer = tumbler.transform.Find("handle").GetComponent<SpriteRenderer>(),
                    Tumbler = tumbler.transform.Find("body/tumbler"),
                    TargetY = TUMBLER_REST_Y,
                    Velocity = Vector3.zero
                };
            }
        }

        void Start() 
        {
            if (this.m_Tumblers==null)
                Generate(5);

            this.m_ClipUnlockRef.LoadAssetAsync<AudioClip>().Completed += handle => this.m_ClipUnlock = handle.Result;
            this.m_ClipHitRef.LoadAssetAsync<AudioClip>().Completed += handle => this.m_ClipHit = handle.Result;
            this.m_ClipFailRef.LoadAssetAsync<AudioClip>().Completed += handle => this.m_ClipFail = handle.Result;
        }

        void OnDestroy()
        {
            this.m_ClipUnlockRef.ReleaseAsset();
            this.m_ClipHitRef.ReleaseAsset();
            this.m_ClipFailRef.ReleaseAsset();
        }

        private Collider2D[] res = new Collider2D[1];
        void Update()
        {
            if (Time.deltaTime==0)
                return;

            if (this.m_Tumblers==null)
                return;

            if (m_InputEnabled)
            {
                float x = Input.GetAxis("Mouse X") + (Input.GetAxis("Horizontal") * Time.deltaTime * 10);
                this.m_PickCollider.transform.position += new Vector3(x * m_PickSpeed, 0, 0);
                this.m_PickCollider.transform.position = new Vector3(
                    Mathf.Clamp(this.m_PickCollider.transform.position.x, this.m_FrontPiece.position.x, this.m_FrontPiece.position.x + (TUMBLER_DIMENSION*this.m_Tumblers.Length)),
                    this.m_PickCollider.transform.position.y,
                    this.m_PickCollider.transform.position.z
                );

                if (Input.GetMouseButtonDown(1))
                {
                    Outcome = OUTCOME_QUIT;
                    m_InputEnabled = false;
                }
            }

            int found = -1;
            for (int i=0; i<this.m_TumblerCount; i++)
            {
                this.m_Tumblers[i].HandleRenderer.color = new Color32(0xCC, 0xB1, 0x86, 0xff);
            }

            if (this.m_PickCollider.OverlapCollider(new ContactFilter2D() { useTriggers = true, useLayerMask = true, layerMask = this.m_HandleLayerMask}, res) > 0)
            {
                string t = res[0].gameObject.name;
                int i;
                if (int.TryParse(t, out i))
                {
                    this.m_Tumblers[i].HandleRenderer.color = Color.green;
                    this.m_PickCollider.transform.localPosition = new Vector3(this.m_PickCollider.transform.localPosition.x, Mathf.Clamp(this.m_PickCollider.transform.localPosition.y - Time.deltaTime * 0.2f, -0.05f, 0f), this.m_PickCollider.transform.localPosition.z);
                    found = i;
                }
            }

            if (found<0)
                this.m_PickCollider.transform.localPosition = new Vector3(this.m_PickCollider.transform.localPosition.x, Mathf.Clamp(this.m_PickCollider.transform.localPosition.y + Time.deltaTime * 0.2f, -0.05f, 0), this.m_PickCollider.transform.localPosition.z);
            else if (Input.GetMouseButtonDown(0) && m_InputEnabled && this.m_SequencePosition < this.m_TumblerCount - 1)
            {
                this.GetComponent<Animator>().SetTrigger("pick");
                AudioSource.PlayClipAtPoint(this.m_ClipHit, Vector3.zero);
                m_SequencePosition++;
                if (this.m_TumblerSequence[this.m_SequencePosition] == found)
                {
                    this.m_Tumblers[found].Root.GetComponent<Animator>().SetTrigger("pick");
                    this.m_Tumblers[found].TargetY =  UnityEngine.Random.Range(-0.5f, 0.5f);
                    this.m_Tumblers[found].MaxSpeed = 10f;
                } else {
                    AudioSource.PlayClipAtPoint(this.m_ClipFail, Vector3.zero);
                    this.m_Tumblers[found].Root.GetComponent<Animator>().SetTrigger("pick-wrong");
                    for (int i=0; i<this.m_TumblerCount; i++) {
                        this.m_Tumblers[i].TargetY = TUMBLER_REST_Y;
                        this.m_Tumblers[i].MaxSpeed = 50;
                    }
                    m_SequencePosition = -1;
                }

                if (m_SequencePosition>=m_TumblerCount-1)
                {
                    Outcome = OUTCOME_UNLOCKED;
                    m_InputEnabled = false;
                    // this.GetComponent<Animator>().SetTrigger("success");
                    // for (int i=0; i<this.m_TumblerCount; i++) {
                    //     this.m_Tumblers[i].Root.GetComponent<Animator>().SetTrigger("success");
                    // }
                    AudioSource.PlayClipAtPoint(this.m_ClipUnlock, Vector3.zero);
                }
            }

            for (int i=0; i<this.m_TumblerCount; i++)
            {
                this.m_Tumblers[i].Tumbler.localPosition = 
                    Vector3.SmoothDamp(
                        this.m_Tumblers[i].Tumbler.localPosition,
                        new Vector3(this.m_Tumblers[i].Tumbler.localPosition.x, this.m_Tumblers[i].TargetY, this.m_Tumblers[i].Tumbler.localPosition.z),
                        ref this.m_Tumblers[i].Velocity,
                        0.1f,
                        this.m_Tumblers[i].MaxSpeed
                    );
            }
        }
        
    }

    public static class ArrayExtensions {
        public static void Fill<T>(this T[] originalArray, T with) {
            for(int i = 0; i < originalArray.Length; i++){
                originalArray[i] = with;
        }
    }  
}
}