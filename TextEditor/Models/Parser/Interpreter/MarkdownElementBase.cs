using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace TextEditor.Models.Parser.Interpreter
{
    public abstract class MarkdownElementBase : IMarkdownElement
    {
        protected readonly string Pattern;

        protected MarkdownElementBase(string pattern)
        {
            Pattern = pattern;
        }

        public abstract Inline? Parse(string text, ref int position);

        protected bool TryMatch(string text, int position, out Match match)
        {
            if (position >= text.Length)
            {
                match = Match.Empty;
                return false;
            }

            match = Regex.Match(text.Substring(position), Pattern);
            return match.Success;
        }
    }
}
