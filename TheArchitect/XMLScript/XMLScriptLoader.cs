
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript
{
    public enum XMLScriptContext {
        Cutscene,
        Exploration
    }

    public class XMLScriptLoader
    {
        public static XMLScriptInstance Load(string resourcePath)
        {
            string path = $"{Application.streamingAssetsPath}/xml/{resourcePath}.xml".Replace("\n","");
            if (!File.Exists(path)) 
            {
                throw new System.Exception($"Can't find '{path}'");
            }

            return _Load(path);
        }

        private static XMLScriptInstance _Load(string path)
        {
            using (StreamReader stream = File.OpenText(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XMLScriptInstance));
                XmlReader xmlReader = XmlReader.Create(stream);
                XMLScriptInstance ci = (XMLScriptInstance)serializer.Deserialize(xmlReader);
                if (ci.Includes!=null)
                    foreach (var i in ci.Includes)
                    {
                        XMLScriptInstance included = _Load(Path.Combine(Path.GetDirectoryName(path), i.Path));
                        ci.IncludeNodes(i, included);
                    }

                return ci;
            }
        }

    }
}