using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{

    public class SFXAction : XMLScriptAction
    {
        [XmlAttribute("clip")]
        public string Clip;
        [XmlAttribute("volume")]
        public float Volume = 1;
        [XmlAttribute("async")]
        public bool Async = false;
        
        private Coroutine m_Coroutine = null;
        private string m_Output = null;

        public override void ResetState()
        {
            this.m_Coroutine = null;
            this.m_Output = null;
        }

        public override string Update(XMLScriptInstance instance, XMLScriptController controller)
        {
            if (this.Async)
                this.m_Output = OUTPUT_NEXT;
                
            if (this.m_Coroutine==null)
                this.m_Coroutine = controller.StartCoroutine(Load(controller));

            return this.m_Output;
        }

        private System.Collections.IEnumerator Load(XMLScriptController controller)
        {
            var assetKey = $"sfx/{this.Clip}";
            var handle = Addressables.LoadAssetAsync<AudioClip>(assetKey);
            
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                float volumeMultiplier = Tags.HasTag(assetKey, Tags.TAG_NSFW) ? GameSettings.GetVolumeNFSW() : 1;

                GameObject tempAudioSource = new GameObject("TempAudio");
                AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
                audioSource.clip = handle.Result;
                audioSource.volume = this.Volume * volumeMultiplier;
                audioSource.spatialBlend = 0.0f;
                audioSource.Play();

                GameObject.Destroy(audioSource.gameObject, handle.Result.length);

                
                yield return new WaitForSeconds(handle.Result.length);
            }
            
            this.m_Output = OUTPUT_NEXT;
        }

    }
}