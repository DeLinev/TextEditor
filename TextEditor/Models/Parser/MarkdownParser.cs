using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using TextEditor.Models.Parser.Interpreter;

namespace TextEditor.Models.Parser
{
    public class MarkdownParser
    {
        private readonly MarkdownDocumentBuilder _builder;

        public MarkdownParser()
        {
            var inlineParser = new InlineParser();
            var blockParser = new BlockParser(inlineParser);
            _builder = new MarkdownDocumentBuilder(blockParser);
        }

        public FlowDocument Parse(string markdownText)
        {
            return _builder.Build(markdownText);
        }
    }


}