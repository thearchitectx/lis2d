using System.Xml.Serialization;
using  TheArchitect.XMLScript.Action;

namespace TheArchitect.XMLScript
{
    public class XMLScriptNode
    {
        
        public string m_Id;
        private string m_DefaultOutput;
        private XMLScriptAction[] m_Actions;
        private int m_CurrentAction = 0;

        [XmlAttribute("id")]
        public string Id 
        {
            set { this.m_Id = value; }
            get { return this.m_Id = this.m_Id != null ? this.m_Id : "node"; }
        }

        [XmlAttribute("out")]
        public string Output 
        {
            set { this.m_DefaultOutput = value; }
            get { return this.m_DefaultOutput; }
        }

        [XmlElement("nop", typeof(XMLScriptAction)),
            XmlElement("debug-log", typeof(DebugLogAction)),
            XmlElement("script-outcome", typeof(ScriptOutcomeAction)),
            XmlElement("node-output", typeof(OutputAction)),
            XmlElement("autosave", typeof(AutosaveAction)),
            XmlElement("obj", typeof(ObjectAction)),
            XmlElement("load-scene-object", typeof(LoadSceneObjectAction)),
            XmlElement("wait", typeof(WaitAction)),
            XmlElement("anim", typeof(AnimateAction)),
            XmlElement("sys", typeof(SystemMessageAction)),
            XmlElement("dlg", typeof(DialogAction)),
            XmlElement("choice", typeof(ChoiceAction)),
            XmlElement("bootstrap", typeof(BootstrapScriptAction)),
            XmlElement("fade-to-black", typeof(FadeToBlackAction)),
            XmlElement("log", typeof(LogAction)),
            XmlElement("if", typeof(IfAction)),
            XmlElement("flag", typeof(SetFlagAction)),
            XmlElement("string", typeof(StringAction)),
        //     XmlElement("trophy", typeof(TrophyAction)),
            XmlElement("sfx", typeof(SFXAction)),
            XmlElement("bgm", typeof(BGMAction)),
        ]
        public XMLScriptAction[] Actions
        {
            set { this.m_Actions = value; }
            get { return this.m_Actions == null ? this.m_Actions = new XMLScriptAction[0] : this.m_Actions; }
        }

        [XmlIgnore]
        public XMLScriptAction CurrentAction
        {
            get { return m_CurrentAction < this.Actions.Length ? this.Actions[m_CurrentAction] : null; }
        }

        public bool HasNextAction()
        {
            return m_CurrentAction+1 < this.Actions.Length;
        }

        public XMLScriptAction NextAction()
        {
            m_CurrentAction++;
            return CurrentAction;
        }

        public void ResetState()
        {
            this.m_CurrentAction = 0;
            foreach (var a in  this.m_Actions)
                a.ResetState();
        }
    }
}