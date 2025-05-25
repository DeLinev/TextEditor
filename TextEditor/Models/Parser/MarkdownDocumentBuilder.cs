using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace TextEditor.Models.Parser
{
    public class MarkdownDocumentBuilder
    {
        private readonly BlockParser _blockParser;

        public MarkdownDocumentBuilder(BlockParser blockParser)
        {
            _blockParser = blockParser;
        }

        public FlowDocument Build(string markdownText)
        {
            if (string.IsNullOrEmpty(markdownText))
                return new FlowDocument();

            var lines = markdownText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var blocks = _blockParser.ParseBlocks(lines);

            var document = new FlowDocument();
            foreach (var block in blocks)
            {
                document.Blocks.Add(block);
            }

            return document;
        }
    }

}
