using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.XMLScript;
using TheArchitect.XMLScript.Model;
using TheArchitect.XMLScript.Action;

namespace TheArchitect.XMLScript.Action
{
    public class InjectAction : XMLScriptAction
    {
        [XmlAttribute("node")]
        public string Node;

        [XmlIgnore]
        private XMLScriptNode InjectedNode = null;

        public override void ResetState()
        {
            InjectedNode = null;
        }

        public override string Update(XMLScriptInstance xmlscript, XMLScriptController controller)
        {
            if (InjectedNode == null)
            {
                int n = xmlscript.FindNode(Node);
                if (n>=0)
                {
                    InjectedNode = xmlscript.Nodes[n];
                    InjectedNode.ResetState();
                    return null;
                }
                else
                {
                    Debug.LogWarning($"Can't find node {Node} {n}");
                    return OUTPUT_NEXT;
                }

            }
            else
            {
                var output = InjectedNode.CurrentAction.Update(xmlscript, controller);
                if (output==OUTPUT_NEXT && InjectedNode.HasNextAction())
                {
                    InjectedNode.NextAction();
                    return null;
                }
                else
                {
                    return output;
                }
            }


        }

    }

}