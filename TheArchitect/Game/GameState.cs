using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TheArchitect.Game
{
    
    [XmlRoot("state")]
    public partial class GameState
    {
        public const string SYSTEM_VERSION = "SYSTEM:VERSION";
        public const string SYSTEM_PLAYTIME = "SYSTEM:PLAYTIME";
        public const string SYSTEM_SCRIPT_PATH_VARIABLE = "SYSTEM:SCRIPT:PATH";
        public const string SYSTEM_SCRIPT_NODE_VARIABLE = "SYSTEM:SCRIPT:NODE";
        public const string SYSTEM_PLAYER_ROT = "SYSTEM:PLAYER:ROT";
        public const string SYSTEM_PLAYER_X = "SYSTEM:PLAYER:X";
        public const string SYSTEM_PLAYER_Y = "SYSTEM:PLAYER:Y";
        public const string SYSTEM_PLAYER_SPAWN = "SYSTEM:PLAYER:SPAWN";

        [XmlElement("flag")]
        public FlagVariable[] Flags = new FlagVariable[0];
        [XmlElement("string")]
        public StringVariable[] Strings = new StringVariable[0];
        [XmlElement("float")]
        public FloatVariable[] Floats = new FloatVariable[0];
    }

    #region Variables definition
    public interface Variable
    {
        string GetName();
        object GetState();
        void SetState(object value);
    }

    public struct FloatVariable : Variable
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("state")]
        public float State;
        
        public string GetName() { return Name; }
        public object GetState() { return State; }
        public void SetState(object value) { this.State = (float) value; }
    }

    public struct FlagVariable : Variable
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("state")]
        public int State;

        public string GetName() { return Name; }
        public object GetState() { return State; }
        public void SetState(object value) { this.State = (int) value; }
    }

    public struct StringVariable : Variable
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("value")]
        public string State;

        public string GetName() { return Name; }
        public object GetState() { return State; }
        public void SetState(object value) { this.State = (string) value; }
    }
    #endregion

}