using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace TheArchitect.Core
{
    [XmlRoot("tags")]
    public class Tags
    {
        public const string TAG_NSFW = "NSFW";
        public const string TAG_MUSIC = "MUSIC";
        public const string FILE_PATH = "xml/asset-tags.xml";
        private static Dictionary<string, HashSet<string>> LoadedAssetsList;

        [XmlElement("tag")]
        public Tag[] TagList;

        public static bool HasTag(string key, string tag)
        {
            if (Tags.LoadedAssetsList == null)
            {
                string path = $"{Application.streamingAssetsPath}/{FILE_PATH}";
                using (FileStream stream = File.OpenRead(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Tags));
                    XmlReader xmlReader = XmlReader.Create(stream);
                    Tags items = (Tags) serializer.Deserialize(xmlReader);

                    Tags.LoadedAssetsList = new Dictionary<string, HashSet<string>>(items.TagList.Length);
                    foreach (var t in items.TagList)
                        Tags.LoadedAssetsList.Add(t.tag, new HashSet<string>(t.keys));
                }

            }

            HashSet<string> keys;
            if (Tags.LoadedAssetsList.TryGetValue(tag, out keys))
                return keys.Contains(key);
            
            return false;
        }
    }

    public class Tag
    {
        [XmlAttribute("name")]
        public string tag;
        [XmlElement("key")]
        public string[] keys;
    }
}