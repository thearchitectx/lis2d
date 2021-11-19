using System.Collections.Generic;
using UnityEngine;

namespace TheArchitect.Core
{

    [CreateAssetMenu(fileName = "Game", menuName = "LIS/Character")]
    public class Character : ScriptableObject
    {
        [SerializeField] public CharacterData Data;
    }

    [System.Serializable]
    public struct CharacterData
    {
        public string DefaultDisplayName;
        public int Age;
        public Color Color;
        public Color ColorContrast;
        public DialogBlip DialogBlip;
    }

    public enum DialogBlip
    {
        male, female, player
    }
}