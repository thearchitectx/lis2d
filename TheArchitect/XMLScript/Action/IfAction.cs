using UnityEngine;
using System.Xml.Serialization;
using TheArchitect.Core;
using TheArchitect.Game;
using TheArchitect.XMLScript.Model;

namespace TheArchitect.XMLScript.Action
{
    public class IfAction : XMLScriptAction
    {
        [XmlElement("check-flag", typeof(CheckFlag)),
            // XmlElement("check-perk", typeof(CheckPerk)),
            // XmlElement("check-stat", typeof(CheckStat)),
            // XmlElement("check-item", typeof(CheckItem)),
            XmlElement("check-text", typeof(CheckText)),
            XmlElement("check-group", typeof(CheckGroupPredicate))
        ]
        public Predicate[] predicates;

        [XmlElement("then")]
        public XMLScriptNode ThenNode;
        [XmlElement("else")]
        public XMLScriptNode ElseNode;

        [XmlIgnore]
        private XMLScriptNode ElectedNode;

        public override void ResetState()
        {
            ElectedNode = null;
        }

        public override string Update(XMLScriptInstance xmlscript, XMLScriptController controller)
        {
            if (ElectedNode == null)
            {
                ElectedNode = Predicate.Resolve(controller.Game, predicates) ? ThenNode : ElseNode;
                ElectedNode?.ResetState();
                return ElectedNode == null ? OUTPUT_NEXT : null;
            }
            else
            {
                var output = ElectedNode.CurrentAction.Update(xmlscript, controller);
                if (output==OUTPUT_NEXT && ElectedNode.HasNextAction())
                {
                    ElectedNode.NextAction();
                    return null;
                }
                else
                {
                    return output;
                }
            }


        }

    }

    public class Predicate
    {
        public virtual bool Resolve(GameContext context)
        {
            return false;
        }

        public static bool Resolve(GameContext context, Predicate[] predicate)
        {
            if (predicate==null)
                return true;
                
            bool b = true;
            foreach (var ifp in predicate)
                b = b && ifp.Resolve(context);

            return b;
        }
    }

    public class CheckFlag : Predicate
    {
        [XmlAttribute("flag")]
        public string Flag;
        [XmlAttribute("inverse")]
        public bool Inverse = false;
        [XmlAttribute("eq")]
        public int Eq = int.MinValue;
        [XmlAttribute("gte")]
        public int Gte = int.MinValue;
        [XmlAttribute("lte")]
        public int Lte = int.MinValue;
        [XmlAttribute("mod")]
        public int Mod = int.MinValue;
        [XmlAttribute("bit-set")]
        public byte BitSet = byte.MaxValue;
        [XmlAttribute("bit-unset")]
        public byte BitUnset = byte.MaxValue;
        [XmlAttribute("eq-str")]
        public string EqStr = null;

        public override bool Resolve(GameContext context)
        {
            var flagValue = context.GetVariable(Flag, 0);
            bool b = false;
            if (Mod > int.MinValue)
                flagValue = flagValue % Mod;
            if (Eq > int.MinValue)
                b = b || flagValue == Eq;
            if (Gte > int.MinValue)
                b = b || flagValue >= Gte;
            if (Lte > int.MinValue)
                b = b || flagValue <= Lte;
            if (BitSet < 32)
                b = b || ( (flagValue & (1 << BitSet)) != 0 );
            if (BitUnset < 32)
                b = b || ( (flagValue & (1 << BitUnset)) == 0 );
            if (!string.IsNullOrEmpty(EqStr))
                b = b || flagValue == ResourceString.ParseToInt(EqStr, context.GetVariable);
            
            b = Inverse ? !b : b;
            #if UNITY_EDITOR
            // Debug.Log($"CHECK-FLAG Flag='{Flag}' Eq='{Eq}' Gte='{Gte}' Lte='{Lte}' Inverse:{Inverse}: {b}");
            #endif
            return b;
        }
    }

    public  class CheckGroupPredicate  : Predicate
    {
        [XmlElement("check-flag", typeof(CheckFlag)),
            // XmlElement("check-perk", typeof(CheckPerk)),
            // XmlElement("check-stat", typeof(CheckStat)),
            // XmlElement("check-item", typeof(CheckItem)),
            XmlElement("check-text", typeof(CheckText)),
            XmlElement("check-group", typeof(CheckGroupPredicate))
        ]
        public Predicate[] predicates;
        [XmlAttribute("op")]
        public string Op;

        public override bool Resolve(GameContext context)
        {
            if (predicates==null || predicates.Length == 0)
                return true;

            bool b = Op == "OR" ? false : true;
            foreach (var p in predicates)
            {
                if (Op == "OR")
                    b |= p.Resolve(context);
                else
                    b &= p.Resolve(context);
            }

            return b;
        }
    }

    // public class CheckItem : Predicate
    // {
    //     [XmlAttribute("item")]
    //     public string Item;
    //     [XmlAttribute("inverse")]
    //     public bool Inverse = false;
    //     [XmlAttribute("eq")]
    //     public int Eq = int.MinValue;
    //     [XmlAttribute("gte")]
    //     public string Gte;
    //     [XmlAttribute("lte")]
    //     public int Lte = int.MinValue;

    //     public override bool Resolve(GameContext context)
    //     {
    //         var itemCount = Resources.Load<Game>(ResourcePaths.SO_GAME).GetItemCount(Item);
    //         bool b = false;
    //         if (Eq > int.MinValue)
    //             b = b || itemCount == Eq;
    //         if (!string.IsNullOrEmpty(Gte))
    //             b = b || itemCount >= ResourceString.ParseToInt(Gte);
    //         if (Lte > int.MinValue)
    //             b = b || itemCount <= Lte;
            
    //         return Inverse ? !b : b;
    //     }
    // }

    // public class CheckStat : Predicate
    // {
    //     [XmlAttribute("char")]
    //     public string Char;
    //     [XmlAttribute("stat")]
    //     public string Stat;
    //     [XmlAttribute("inverse")]
    //     public bool Inverse = false;
    //     [XmlAttribute("eq")]
    //     public int Eq = int.MinValue;
    //     [XmlAttribute("eq-str")]
    //     public string EqStr = null;
    //     [XmlAttribute("gte")]
    //     public int Gte = int.MinValue;
    //     [XmlAttribute("gte-than-stat")]
    //     public string GteStat = null;
    //     [XmlAttribute("lte")]
    //     public int Lte = int.MinValue;

    //     public override bool Resolve()
    //     {
    //         Game game = Resources.Load<Game>(ResourcePaths.SO_GAME);
    //         Character character = Resources.Load<Character>($"{ResourcePaths.SO_CHARACTERS}/{Char}");
    //         var stat = game.GetCharacterStat(character, Stat);
    //         bool b = false;
    //         if (Eq > int.MinValue)
    //             b = b || stat == Eq;
    //         if (Gte > int.MinValue)
    //             b = b || stat >= Gte;
    //         if (Lte > int.MinValue)
    //             b = b || stat <= Lte;
    //         if (!string.IsNullOrEmpty(GteStat)) {
    //             var v = game.GetCharacterStat(character, GteStat);
    //             b = b || stat >= v;
    //         }

    //         if (!string.IsNullOrEmpty(EqStr))
    //             b = b || stat == ResourceString.ParseToInt(EqStr);

            
    //         return Inverse ? !b : b;
    //     }
    // }

    // public class CheckPerk : Predicate
    // {
    //     [XmlAttribute("perk")]
    //     public string Perk;
    //     [XmlAttribute("inverse")]
    //     public bool Inverse = false;

    //     public override bool Resolve()
    //     {
    //         bool b = Resources.Load<Game>(ResourcePaths.SO_GAME).HasActivePerk(Perk);
    //         return Inverse ? !b : b;
    //     }
    // }

    public class CheckText : Predicate
    {
        [XmlAttribute("text")]
        public string Text;
        [XmlAttribute("ref")]
        public bool IsRef;
        [XmlAttribute("eq")]
        public string Eq = null;
        [XmlAttribute("neq")]
        public string Neq = null;
        [XmlAttribute("match")]
        public string Matches = null;

        public override bool Resolve(GameContext context)
        {
            var textValue = context.GetVariable( IsRef ? context.GetVariable(Text, "") : Text, "" );
            bool b = false;
            if (Eq == "#EMPTY")
                b = b || string.IsNullOrEmpty(textValue);
            else if (!string.IsNullOrEmpty(Eq))
                b = b || textValue == ResourceString.Parse(Eq, context.GetVariable);

            if (Neq == "#EMPTY")
                b = b || !string.IsNullOrEmpty(textValue);
            else if (!string.IsNullOrEmpty(Neq))
                b = b || textValue != ResourceString.Parse(Neq, context.GetVariable);

            if (!string.IsNullOrEmpty(Matches))
            {
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(Matches);
                b = b || r.IsMatch(textValue);
            }

            return b;
        }
    }
}