using System;
using UnityEngine;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class XMLScriptAction
    {
        public const string OUTCOME_DESTROY_OBJECT = "_destroyObject";
        public const string OUTCOME_DESTROY_CONTROLLER = "_destroyController";
        public const string OUTCOME_DESTROY_PARENT = "_destroyParent";
        public const string OUTPUT_END = "_end";
        public const string OUTPUT_NEXT = "_next";
        
        public virtual void ResetState()
        {
            
        }

        public virtual string Update(XMLScriptInstance cutscene, XMLScriptController controller)
        {
            return OUTPUT_NEXT;
        }

        public virtual object Valid(XMLScriptInstance cutscene)
        {
            return null;
        }

        protected virtual void LogIf(bool exp, string msg)
        {
            if (exp)
            {
                Debug.Log($"{this.GetType().Name}: {msg}");
            }
        }

    }
}