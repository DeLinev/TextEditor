using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace TextEditor.Models.Parser.Interpreter
{
    public class BoldElement : MarkdownElementBase
    {
        public BoldElement() : base(@"^\*\*(.+?)\*\*") { }

        public override Inline? Parse(string text, ref int position)
        {
            if (TryMatch(text, position, out var match))
            {
                position += match.Index + match.Length;
                return new Bold(new Run(match.Groups[1].Value));
            }
            return null;
        }
    }
}