using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{

    public class BGMAction : XMLScriptAction
    {
        private const string DEFAULT_OBJECT_NAME = "_BGMAction";

        [XmlAttribute("loop")]
        public string Loop;
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("volume")]
        public float Volume = 1;
        [XmlAttribute("fade-speed")]
        public float FadeSpeed = 1;
        [XmlAttribute("on-root")]
        public bool OnRoot = false;

        private Coroutine m_Coroutine = null;
        
        public override void ResetState()
        {
            this.m_Coroutine = null;
        }

        public override string Update(XMLScriptInstance instance, XMLScriptController controller)
        {
            if (this.m_Coroutine == null)
                this.m_Coroutine = controller.StartCoroutine(Load(controller));
            
            return OUTPUT_NEXT;
        }

        private System.Collections.IEnumerator Load(XMLScriptController controller)
        {
            var t = FindClipObject(controller);
            AudioSource existingSource = t!=null ? t.GetComponent<AudioSource>() : null;

            if (string.IsNullOrEmpty(Loop))
            {
                if (existingSource !=null)
                {
                    while (existingSource.volume != this.Volume)
                    {
                        existingSource.volume = Mathf.MoveTowards(existingSource.volume, this.Volume, Time.deltaTime * this.FadeSpeed);
                        yield return new WaitForEndOfFrame();
                    }

                    if (this.Volume == 0)
                        GameObject.Destroy(existingSource.gameObject);
                }
            }
            else
            {
                if (existingSource != null)
                {
                    while (existingSource.volume != 0)
                    {
                        existingSource.volume = Mathf.MoveTowards(existingSource.volume, 0, Time.deltaTime * this.FadeSpeed);
                        yield return new WaitForEndOfFrame();
                    }

                    GameObject.Destroy(existingSource.gameObject);
                }

                var handle = Addressables.LoadAssetAsync<AudioClip>($"sfx/{this.Loop}");
        
                yield return handle;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject tempAudioSource = new GameObject(string.IsNullOrEmpty(Name) ? DEFAULT_OBJECT_NAME : Name);
                    if (!OnRoot)
                        tempAudioSource.transform.SetParent(controller.transform);

                    AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
                    audioSource.clip = handle.Result;
                    audioSource.volume = 0;
                    audioSource.spatialBlend = 0.0f;
                    audioSource.loop = true;
                    audioSource.Play();

                    while (audioSource.volume != this.Volume)
                    {
                        audioSource.volume = Mathf.MoveTowards(audioSource.volume, this.Volume, Time.deltaTime * this.FadeSpeed);
                        yield return new WaitForEndOfFrame();
                    }
                }

            }
        }

        private Transform FindClipObject(XMLScriptController controller)
        {
            if (OnRoot)
            {
                var g = GameObject.Find((string.IsNullOrEmpty(Name) ? DEFAULT_OBJECT_NAME : Name));
                return  g != null ? g.transform : null;
            }
            else
            {
                return controller.FindProxy((string.IsNullOrEmpty(Name) ? DEFAULT_OBJECT_NAME : Name));
            }
        }
    }
}