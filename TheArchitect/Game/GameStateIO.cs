using System.ComponentModel;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using TheArchitect;

namespace TheArchitect.Game
{
    public struct GameStateIO
    {
        public const string SAVE_SUB_DIRECTORY = "saves";
        public const string STATE_FILE_NAME = "state.xml";
        public const string LABEL_FILE_NAME = "label.txt";
        public const string SCREEN_FILE_NAME = "screen.jpg";
        public const string PICS_FOLDER = "pics";

        public static void Rename(string root, string slot, string label)
        {
            Directory.CreateDirectory(GetSlotPath(root, slot));
            
            string labelFilePath = $"{GetSlotPath(root, slot)}/{LABEL_FILE_NAME}";
            File.WriteAllText(labelFilePath, label);
        }

        public static IEnumerator SaveScreenshot(string rootDir, string slot)
        {
            string file = $"{GetSlotPath(rootDir, slot)}/{SCREEN_FILE_NAME}";

            return ScreenshotUtils.TakeScreenshot(256, tex => {
                var data = tex.EncodeToJPG(80);
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, args) => {
                    File.WriteAllBytes( $"{file}" , data);
                };
                worker.RunWorkerCompleted += (sender, args) => Debug.Log($"Saving screenshot at {file} {args.Error?.Message}");
                worker.RunWorkerAsync();
            });
        }
        
        public static string Save(GameState state, string rootDir, string slot, string label, string gameVersion)
        {
            Directory.CreateDirectory(GetSlotPath(rootDir, slot));

            string saveFilePath = $"{GetSlotPath(rootDir, slot)}/{STATE_FILE_NAME}";
            string labelFilePath = $"{GetSlotPath(rootDir, slot)}/{LABEL_FILE_NAME}";

            using (StreamWriter stream = File.CreateText(saveFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameState));
                XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true });
                serializer.Serialize(xmlWriter, state);
            }

            File.WriteAllText(labelFilePath, label);
            return saveFilePath;
        }

        public static string GetSlotPath(string root, string slot)
        {
            return $"{root}/{SAVE_SUB_DIRECTORY}/{slot}";
        }

        public static GameState Load(string root, string slot)
        {
            string saveFilePath = $"{GetSlotPath(root, slot)}/{STATE_FILE_NAME}";

            Directory.CreateDirectory(GetSlotPath(root, slot));

            using (StreamReader stream = File.OpenText(saveFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameState));
                XmlReader xmlReader = XmlReader.Create(stream);
                GameState gs = serializer.Deserialize(xmlReader) as GameState;
                return gs;
            }
        }

        public static string LoadLabel(string root, string slot)
        {
            string labelFilePath = $"{GetSlotPath(root, slot)}/{LABEL_FILE_NAME}";
            
            return File.Exists(labelFilePath)
                ? File.ReadAllText(labelFilePath)
                : null;
        }

        public static byte[] LoadImage(string root, string slot)
        {
            string labelFilePath = $"{GetSlotPath(root, slot)}/{SCREEN_FILE_NAME}";
            
            return File.Exists(labelFilePath)
                ? File.ReadAllBytes(labelFilePath)
                : new byte[0];
        }

        public static string LoadLastWrite(string root, string slot)
        {
            string labelFilePath = $"{GetSlotPath(root, slot)}/{STATE_FILE_NAME}";
            System.DateTime d = File.GetLastWriteTime(labelFilePath);
            return File.Exists(labelFilePath)
                ? d.ToString("f") : "";
        }
    }
}