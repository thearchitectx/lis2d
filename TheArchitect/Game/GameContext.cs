using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TheArchitect.Game
{

    [System.Serializable]
    public class AssetReferenceGameContext : AssetReferenceT<GameContext>
    {
        public AssetReferenceGameContext(string guid) : base(guid) { }
    }

    [CreateAssetMenu(fileName = "Game", menuName = "LIS/GameContext")]
    public class GameContext : ScriptableObject
    {
        [SerializeField] public string DefaultStartScript;
        [NonSerialized] private Dictionary<string, Variable> m_StateIndex;
        [NonSerialized] private int m_StateChangeCount;

        void OnEnable()
        {
            Debug.Log("GameContext OnEnable");
            this.m_StateIndex = new Dictionary<string, Variable>();
        }

        public int StateChangeCount {
            get { return m_StateChangeCount; }
        }

        public T GetVariable<T>(string name, T defaultValue)
        {
            Variable v;
            return this.m_StateIndex.TryGetValue(name, out v)
                ? (T) v.GetState()
                : defaultValue;
        }

        public object GetVariable(string name)
        {
            Variable v;
            return this.m_StateIndex.TryGetValue(name, out v)
                ? v.GetState()
                : null;
        }

        public void SetVariable(string name, int state) { this.SetVariableEx(name, state); }
        public void SetVariable(string name, float state) { this.SetVariableEx(name, state); }
        public void SetVariable(string name, string state) { this.SetVariableEx(name, state); }

        private void SetVariableEx(string name, object state)
        {
            if (this.m_StateIndex.ContainsKey(name))
            {
                this.m_StateIndex[name].SetState(state);
                m_StateChangeCount++;
            }
            else
            {
                Variable v;
                if (state is int) v = new FlagVariable() { Name = name, State = (int) state};
                else if (state is float) v = new FloatVariable()  { Name = name, State = (float) state};
                else if (state == null || state is string) v = new StringVariable()  { Name = name, State = (string) state};
                else throw new System.Exception("Unexpected variable type");

                this.m_StateIndex[name] = v;
                m_StateChangeCount++;
            }

            #if UNITY_EDITOR
            if (!name.StartsWith("SYSTEM"))
                Debug.Log($"SetVariable {name}={state}  #{m_StateChangeCount}");
            #endif
        }

        public void UnsetVariable(string name) 
        {
            if (this.m_StateIndex.ContainsKey(name)) this.m_StateIndex.Remove(name);
        }

        public GameState BuildStateInstance()
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                this.SetVariable(GameState.SYSTEM_PLAYER_X, player.transform.localPosition.x);
                this.SetVariable(GameState.SYSTEM_PLAYER_Y, player.transform.localPosition.y);
            }
            if (!this.m_StateIndex.ContainsKey(GameState.SYSTEM_VERSION))
            {
                this.SetVariable(GameState.SYSTEM_VERSION, Application.version);
            }

            return new GameState() {
                Flags = this.m_StateIndex.Values.Where( v => v is FlagVariable ).Select( v => (FlagVariable) v).ToArray(),
                Strings = this.m_StateIndex.Values.Where( v => v is StringVariable ).Select( v => (StringVariable) v).ToArray(),
                Floats = this.m_StateIndex.Values.Where( v => v is FloatVariable ).Select( v => (FloatVariable) v).ToArray()
            };
        }

        public void ApplyStateInstance(GameState gameState)
        {
            this.m_StateIndex.Clear();
            if (gameState.Flags!=null)
                foreach (var f in gameState.Flags) this.m_StateIndex.Add(f.Name, f);
            if (gameState.Floats!=null)
                foreach (var f in gameState.Floats) this.m_StateIndex.Add(f.Name, f);
            if (gameState.Strings!=null)
                foreach (var s in gameState.Strings) this.m_StateIndex.Add(s.Name, s);
            this.m_StateChangeCount = 0;
        }
    }

}