using UnityEngine;

namespace TheArchitect.SceneObjects
{
    public class SceneObject : MonoBehaviour
    {
        public const string OUTCOME_END = "OUTCOME_END";
        
        [System.NonSerialized] public string Outcome;

        public void SetOutcome(string o)
        {
            this.Outcome = o;
        }
        
        public void ClearOutcome()
        {
            Outcome = null;
        }

    }
    
}