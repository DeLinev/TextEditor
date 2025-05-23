using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;

namespace TextEditor.Models.Parser.Interpreter
{
    public class BoldItalicElement : MarkdownElementBase
    {
        public BoldItalicElement() : base(@"^\*\*\*(.+?)\*\*\*") { }

        public override Inline? Parse(string text, ref int position)
        {
            if (TryMatch(text, position, out var match))
            {
                position += match.Index + match.Length;
                return new Run(match.Groups[1].Value)
                {
                    FontWeight = FontWeights.Bold,
                    FontStyle = FontStyles.Italic
                };
            }
            return null;
        }
    }
}