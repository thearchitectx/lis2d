using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Model;
using TheArchitect.Core;

namespace TheArchitect.XMLScript.Action
{
    public class LoadSceneObjectAction : XMLScriptAction
    {
        [XmlAttribute("key")]
        public string Key = null;
        [XmlAttribute("parent")]
        public string Parent = null;
        [XmlAttribute("rename")]
        public string Rename = null;
        [XmlAttribute("proxy")]
        public string Proxy = null;
        [XmlAttribute("active")]
        public bool Active = true;
        [XmlAttribute("y-pos")]
        public float YPos = float.NaN;
        [XmlAttribute("world-position-stays")]
        public bool WorldPositionStays = false;
        [XmlAttribute("ignore-if-exists")]
        public bool IgnoreIfExists = false;
        [XmlAttribute("destroy")]
        public float Destroy = float.NaN;
        [XmlElement("message")]
        public ObjectMessage[] Messages = null;

        private Coroutine m_LoadingCoroutine;
        private string m_Output;

        public override void ResetState()
        {
            this.m_LoadingCoroutine = null;
            this.m_Output = null;
        }

        public override string Update(XMLScriptInstance cutscene, XMLScriptController controller)
        {
            if (this.m_LoadingCoroutine == null)
            {
                this.m_LoadingCoroutine = controller.StartCoroutine(this.LoadAsync(controller));
            }

            return this.m_Output;
        }

        IEnumerator LoadAsync(XMLScriptController controller)
        {
            Transform p = string.IsNullOrEmpty(this.Parent)
                ? controller.transform
                : controller.FindProxy(ResourceString.Parse(this.Parent, controller.Game.GetVariable));

            if (this.IgnoreIfExists && p.Find(string.IsNullOrEmpty(this.Rename) ? this.Key : this.Rename) != null)
                yield break;

            var handle = Addressables.InstantiateAsync(this.Key, p, WorldPositionStays);
            handle.Completed += (h) => {
                GameObject instance = handle.Result;
                CheckCamNoisePref(instance);
                instance.name = string.IsNullOrEmpty(this.Rename) ? this.Key : this.Rename;

                if (!string.IsNullOrEmpty(this.Proxy))
                    controller.AddProxy(this.Proxy, instance.transform);

                if (!float.IsNaN(this.YPos))
                    instance.transform.position = new Vector3(instance.transform.position.x, this.YPos, instance.transform.position.z);

                ObjectAction.SendMessages(this.Messages, instance, controller.Game.GetVariable);

                instance.SetActive(this.Active);
            };

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Addressable not found: {this.Key}");
            }

            this.m_Output = OUTPUT_NEXT;

            if (!float.IsNaN(this.Destroy)) 
            {
                yield return new WaitForSeconds(this.Destroy);
                Addressables.ReleaseInstance(handle.Result);
            }

        }

        private void CheckCamNoisePref(GameObject obj)
        {
            if (GameSettings.GetDisableCameraNoise())
            {
                var cams = obj.transform.GetComponentsInChildren<Cinemachine.CinemachineVirtualCamera>();
                foreach (var cam in cams)
                {
                    var noise = cam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
                    if (noise!=null) {
                        noise.m_AmplitudeGain = 0;
                        noise.m_FrequencyGain = 0;
                    }
                }
            }
        }

    }
}