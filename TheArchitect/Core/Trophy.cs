using UnityEngine;
using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace TheArchitect.Core
{
    [XmlRoot("trophies")]
    public class Trophies
    {

        public const string FILE_PATH = "xml/trophies.xml";
        private static Trophy[] LoadedTrophyList;

        [XmlElement("trophy")]
        public Trophy[] TrophyList;

        public static Trophy Get(string id)
        {
            return GetTrophies().First(t => t.Id == id);
        }

        public static Trophy[] GetTrophies()
        {
            if (Trophies.LoadedTrophyList == null)
            {
                string path = $"{Application.streamingAssetsPath}/{FILE_PATH}";
                using (FileStream stream = File.OpenRead(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Trophies));
                    XmlReader xmlReader = XmlReader.Create(stream, new XmlReaderSettings() {  });
                    Trophies ts = (Trophies)serializer.Deserialize(xmlReader);

                    Trophies.LoadedTrophyList = ts.TrophyList.Select(t => { 
                        t.Description = t.Description.Trim(new char[]{' ', '\n'});
                        return t; 
                    }).ToArray();
                }
            }

            return Trophies.LoadedTrophyList;
        }

        public static void WipeUnlockData() {
            var list = Trophies.GetTrophies();
            
            foreach (var t in list) 
                PlayerPrefs.DeleteKey(t.PlayerPrefKey);


            PlayerPrefs.Save();
        }

    }

    public class Trophy
    {

        [XmlAttribute("id")]
        public string Id;
        [XmlAttribute("label")]
        public string Label;
        [XmlAttribute("icon")]
        public string Icon;
        [XmlAttribute("icon-label")]
        public string IconLabel;
        [XmlText]
        public string Description;

        public string PlayerPrefKey 
        {
            get { return $"TROPHY-{Id.ToUpper()}"; }
        }

        public bool Unlocked
		{
			get { return PlayerPrefs.HasKey(this.PlayerPrefKey); }
		}

		public bool Unlock()
		{
			if (!this.Unlocked) {
				PlayerPrefs.SetString(this.PlayerPrefKey, DateTime.Now.ToString("U")  );
				PlayerPrefs.Save();
				return true;
			}
			return false;
		}

        public string GetUnlockDate()
		{
			return UnityEngine.PlayerPrefs.GetString(this.PlayerPrefKey, "");
		}

    }
}