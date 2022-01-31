using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class FadeToBlackAction : XMLScriptAction
    {
        public const string DEFAULT_NAME = "__fade-to-black";
        [XmlAttribute("speed")]
        public float Speed = 1f;
        [XmlAttribute("keep")]
        public bool Keep = false;
        [XmlAttribute("async")]
        public bool Async = false;
        [XmlAttribute("layer")]
        public int SortingOrder = int.MaxValue;
        [XmlAttribute("fill")]
        public bool Fill = false;

        private Coroutine m_LoadCoroutine;
        private string m_Output = null;

        public override void ResetState()
        {
            this.m_Output = null;
            this.m_LoadCoroutine = null;
        }

        public override string Update(XMLScriptInstance cutscene, XMLScriptController controller)
        {
            if (this.m_LoadCoroutine == null)
            {
                this.m_LoadCoroutine = controller.StartCoroutine(Load(controller));
            }
            
            return this.m_Output;
        }

        private System.Collections.IEnumerator Load(XMLScriptController controller)
        {
            var existing = controller.transform.Find(DEFAULT_NAME);
            System.Action releaseExisting = () => {
                if (existing!=null)
                    if (!Addressables.ReleaseInstance(existing.gameObject))
                        GameObject.Destroy(existing.gameObject);
            };

            if (this.Speed != 0)
            {
                var handle = Addressables.InstantiateAsync("fade-to-black.prefab", controller.transform);

                yield return handle;

                releaseExisting();

                var ftb = handle.Result;
                ftb.name = DEFAULT_NAME;
                ftb.GetComponent<Canvas>().sortingOrder = this.SortingOrder;
                Image image = ftb.GetComponentInChildren<Image>();

                float target = this.Speed < 0 ? 1 : 0;
                if (Fill)
                {
                    // Fill transition
                    image.fillMethod = Image.FillMethod.Radial360;
                    image.fillOrigin = (int) ( this.Speed < 0 ? Image.Origin360.Bottom : Image.Origin360.Top );
                    image.fillClockwise = this.Speed < 0;
                    image.color = new Color(0, 0, 0, 1);

                    image.fillAmount = target;

                    if (this.Async)
                        this.m_Output = OUTPUT_NEXT;

                    while ( target >= 0 && target <= 1)
                    {
                        image.fillAmount = target += Time.deltaTime * this.Speed;
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    // Fade transition
                    image.fillAmount = 1;
                    image.color = new Color(0, 0, 0, target);

                    if (this.Async)
                        this.m_Output = OUTPUT_NEXT;

                    while ( target >= 0 && target <= 1)
                    {
                        image.color = new Color(0, 0, 0, target += Time.deltaTime * this.Speed);
                        yield return new WaitForEndOfFrame();
                    }
                }

                if (!this.Keep)
                    Addressables.ReleaseInstance(ftb);
            }
            else
            {
                releaseExisting();
            }

            this.m_Output = OUTPUT_NEXT;
        }

    }
    
}