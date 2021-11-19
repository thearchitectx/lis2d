using UnityEngine;
using System.Text.RegularExpressions;

namespace TheArchitect.Core
{
    public class ResourceString
    {
        public const string VAR = "VAR";
        public const string RANDOM = "RND";
        public const string VERSION = "VERSION";

        private static Regex regexVariable = new Regex(@"\${([^\${}]+)}*");
        private static Regex regexEmphasis = new Regex(@"\*([^\*]+)\**");
        private static Regex regexSmall = new Regex(@"\|([^\*]+)\|*");
        private static Regex regexLineSpaceDiscard = new Regex(@"^[\s]*|\n\s*|[\s]+$");

        public static int ParseToInt(string s, System.Func<string, object> context)
        {
            string sp = Parse(s, context);
            int o = 0;
            return int.TryParse(sp, out o)
                ? o : 0;
        }

        public static string Parse(string s, System.Func<string, object> context)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            string finalString = s;

            foreach (Match match in regexVariable.Matches(s))
            {
                if (match.Groups.Count < 2)
                {
                    Debug.LogWarning($"Can't find Regex Group on {match.Value}'");
                    continue;
                }

                string[] path = match.Groups[1].Value.Split(new char[] {'.'});

                if (path[0]==RANDOM)
                {
                    finalString = finalString.Replace(
                        match.Value,
                        $"{(UnityEngine.Random.value * 1000000):0000000}"
                    );
                }
                else if (path[0]==VERSION)
                {
                    finalString = finalString.Replace(match.Value, Application.version);
                }
                else if (path[0]==VAR)
                {
                    finalString = finalString.Replace(
                        match.Value,
                        AsString(context(path[1]))
                    );
                }
                else
                {
                    finalString = finalString.Replace(
                        match.Value,
                        AsString(context(path[0]))
                    );
                }
            }

            foreach (Match match in regexEmphasis.Matches(finalString))
            {
                if (match.Groups.Count > 1)
                {
                    finalString = finalString.Replace(
                        match.Value,
                        $"<color=cyan>{match.Groups[1].Value}</color>"
                    );
                }
            }
            
            foreach (Match match in regexSmall.Matches(finalString))
            {
                if (match.Groups.Count > 1)
                {
                    finalString = finalString.Replace(
                        match.Value,
                        $"<i><color=#888><size=12>{match.Groups[1].Value}</size></color></i>".Replace("|", "")
                    );
                }
            }

            return regexLineSpaceDiscard.Replace(finalString, "")
                .Replace("\\n", "\n")
                .Replace("<br>", "\n");
        }


        static string AsString(object value)
        {
            return (value ?? "####").ToString();
        }
    }
}