using UnityEngine;
using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace TheArchitect.Core
{
    [XmlRoot("items")]
    public class Items
    {

        public const string FILE_PATH = "xml/items.xml";
        private static Item[] LoadedItemsList;

        [XmlElement("item")]
        public Item[] ItemList;

        public static Item Get(string id)
        {
            return GetItems().First(t => t.Id == id);
        }

        public static Item[] GetItems()
        {
            if (Items.LoadedItemsList == null)
            {
                string path = $"{Application.streamingAssetsPath}/{FILE_PATH}";
                using (FileStream stream = File.OpenRead(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Items));
                    XmlReader xmlReader = XmlReader.Create(stream);
                    Items items = (Items) serializer.Deserialize(xmlReader);

                    Items.LoadedItemsList = items.ItemList;
                }
            }

            return Items.LoadedItemsList;
        }

    }

    public class Item
    {
        [XmlAttribute("id")]
        public string Id;
        [XmlAttribute("label")]
        public string Label;
        [XmlAttribute("icon")]
        public string Icon;
        [XmlAttribute("price")]
        public int Price;
        [XmlText]
        public string Description;
    }
}