using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TheArchitect.Game;
using TheArchitect.XMLScript.Model;
using TheArchitect.XMLScript.Action;

namespace TheArchitect.XMLScript
{
    public class XMLScriptController : UnityEngine.MonoBehaviour
    {
        [SerializeField] [TextArea] private string m_ScriptPath;
        [SerializeField] private string m_StartNode;
        [SerializeField] private GameObjectProxy[] m_Proxies = new GameObjectProxy[0];
        [SerializeField] private string[] m_ScriptArgs = new string[0];
        [SerializeField] private GameContext m_GameContext;
        [SerializeField] public UnityEvent<string> OnFinished = new UnityEvent<string>();

        private XMLScriptInstance m_Instance;

        public string ScriptPath { get { return this.m_ScriptPath; } }
        public GameContext Game { get { return this.m_GameContext; } }

        public void Configure(string scriptPath, string startNode, GameContext context)
        {
            this.m_ScriptPath = scriptPath;
            this.m_GameContext = context;
            this.m_StartNode = startNode;
            this.Reset();
        }

        public void Reset()
        {
            if (this.m_Instance != null)
                this.m_Instance.Reset();
                
            if (this.m_ScriptArgs != null)
                for (int i =0; i < this.m_ScriptArgs.Length; i++)
                    Game.SetVariable($"ARG:{i}", this.m_ScriptArgs[i]);
        }

        public void Reload()
        {
            this.m_Instance = null;
            this.OnFinished.RemoveAllListeners();
            StartCoroutine(_Reload());
        }

        public System.Collections.IEnumerator _Reload()
        {
            XMLScriptInstance result = null;
            var worker = new System.ComponentModel.BackgroundWorker();
            worker.DoWork += (sender, args) => args.Result = XMLScriptLoader.Load(this.m_ScriptPath);
            worker.RunWorkerCompleted += (sender, args) => result = args.Result as XMLScriptInstance;
            worker.RunWorkerAsync();

            while (worker.IsBusy)
                yield return new WaitForEndOfFrame();

            if (this.m_ScriptArgs != null)
                for (int i =0; i < this.m_ScriptArgs.Length; i++)
                    Game.SetVariable($"ARG:{i}", this.m_ScriptArgs[i]);

            this.m_Instance = result;
            if (!string.IsNullOrEmpty(this.m_StartNode))
                this.m_Instance.JumpToNode(this.m_StartNode);
        }

        /**
        * Start 
        */
        void Start()
        {
            Reload();
        }

        /**
        * Update 
        */
        void Update()
        {
            if (Time.deltaTime == 0 || this.m_Instance == null)
                return; 

            if (this.m_Instance != null)
            {
                if (this.m_Instance.Finished)
                {
                    var outcome = this.m_Instance.Outcome;

                    if (this.m_ScriptArgs != null)
                        for (int i =0; i < this.m_ScriptArgs.Length; i++)
                            Game.UnsetVariable($"ARG:{i}");
                    
                    this.OnFinished.Invoke(this.m_Instance.Outcome);
                    this.OnFinished.RemoveAllListeners();

                    if (outcome == XMLScriptAction.OUTCOME_DESTROY_OBJECT) 
                        Destroy(this.gameObject, 1);
                    else if (outcome == XMLScriptAction.OUTCOME_DESTROY_CONTROLLER)
                        Destroy(this, 1);
                    else if (outcome == XMLScriptAction.OUTCOME_DESTROY_PARENT)
                        Destroy(this.transform.parent.gameObject, 1);
                } else {
                    this.m_Instance.Update(this);
                }
            }
            
        }

        public void AddProxy(string name, Transform target)
        {
            foreach (var p in this.m_Proxies)
                if (p.Name == name)
                {
                    p.Target = target;
                    return;
                }

            System.Array.Resize(ref this.m_Proxies, this.m_Proxies.Length + 1);
            this.m_Proxies[this.m_Proxies.GetUpperBound(0)] = new GameObjectProxy() { Name = name, Target = target};
        }

        public Transform FindProxy(string name)
        {
            if (name == null)
                return null;
            if (name == "this")
                return this.transform;
            if (name == "_parent")
                return this.transform.parent;
                
            List<GameObjectProxy> foundProxies = new List<GameObjectProxy>();
            foreach (var p in this.m_Proxies)
                if (p.Name == name)
                    foundProxies.Add(p);
            
            if (foundProxies.Count==1)
                return foundProxies[0].Target;
            else
            {
                // Return the first active if more than one is found
                foreach (var p in foundProxies)
                    if (p.Target.gameObject.activeSelf)
                        return p.Target;
            }

            if (name.StartsWith("#"))
                return GameObject.FindGameObjectWithTag(name.Substring(1))?.transform;

            if (name.StartsWith("/"))
                return GameObject.Find(name.Substring(1))?.transform;

            if (name.StartsWith("../"))
                return transform.parent.Find(name.Substring(3))?.transform;

            return this.transform.Find(name);
        }

    }

    [System.Serializable]
    public class GameObjectProxy
    {
        public string Name;
        public Transform Target;
    }

}

