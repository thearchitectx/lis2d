using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.XMLScript.Action;

namespace TheArchitect.XMLScript.Model
{
    public class XMLScriptInclude
    {
        [XmlAttribute("node-prefix")]
        public string Prefix;
        [XmlText]
        public string Path;
    }

    [XmlRoot("xml-script")]
    public class XMLScriptInstance 
    {
        [XmlArray("nodes"), XmlArrayItem("n")]
        public XMLScriptNode[] Nodes { get; set; }
        [XmlElement("include")]
        public XMLScriptInclude[] Includes { get; set; }

        [XmlIgnore] public string Outcome;
        [XmlIgnore] private int m_CurrentNode;
        [XmlIgnore] public bool Finished
        {
            get { return this.Nodes != null && m_CurrentNode >= this.Nodes.Length; }
        }
        [XmlIgnore] public XMLScriptNode CurrentNode 
        {
            get { return m_CurrentNode < this.Nodes.Length ? this.Nodes[m_CurrentNode] : null; } 
        }

        ///
        ///
        ///
        public void Reset()
        {
            this.Outcome = null;
            this.m_CurrentNode = 0;
            foreach (var n in this.Nodes)
                n.ResetState();
        }

        ///
        ///
        ///
        public void IncludeNodes(XMLScriptInclude include, XMLScriptInstance loadedInclude)
        {
            var n = new XMLScriptNode[Nodes.Length + loadedInclude.Nodes.Length];
            Nodes.CopyTo(n, 0);
            loadedInclude.Nodes.CopyTo(n, Nodes.Length);

            for (var i = Nodes.Length; i<n.Length; i++)
                n[i].m_Id = include.Prefix + loadedInclude.Nodes[i-Nodes.Length].m_Id;

            this.Nodes = n;
        }


        ///
        ///
        ///
        public void Update(XMLScriptController controller)
        {
            if (Time.deltaTime==0)
            {
                return;
            }
                
            string output = null;
            do {
                try
                {
                    output = CurrentNode.CurrentAction.Update(this, controller);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                    output = XMLScriptAction.OUTPUT_NEXT;
                }


                if (output == XMLScriptAction.OUTPUT_END)
                {
                    this.m_CurrentNode = this.Nodes.Length;
                    return;
                }
                else if (output == XMLScriptAction.OUTPUT_NEXT)
                {
                    if (CurrentNode.HasNextAction())
                    {
                        CurrentNode.NextAction();
                    }
                    else if (CurrentNode.Output != null)
                    {
                        JumpToNode(CurrentNode.Output);
                    }
                    else
                    {
                        this.m_CurrentNode = this.Nodes.Length;
                    }
                }
                else if (output != null)
                {
                    JumpToNode(output);
                }

            } while (output != null && !Finished);
        }

        /// JumpToNode
        public bool JumpToNode(string id)
        {
            int n = FindNode(id);
            if (n>-1)
            {
                this.m_CurrentNode = n;
                this.Nodes[n].ResetState();
                return true;
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Node Id not found: '{id}'");
                this.m_CurrentNode = Nodes.Length;
                return false;
            }
        }

        /// FindNode
        public int FindNode(string id)
        {
            for (var i = 0; i < Nodes.Length; i++)
            {
                if (Nodes[i].Id == id) 
                    return i;
            }
            return -1;
        }


    }

}