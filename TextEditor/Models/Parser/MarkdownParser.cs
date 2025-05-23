using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using TextEditor.Models.Parser.Interpreter;

namespace TextEditor.Models.Parser
{
    public class MarkdownParser
    {
        private const string HeaderPattern = @"^(#{1,6})\s+(.+)$";
        private const string ListSymbol = "- ";
        private const int HeaderBaseSize = 26;

        private readonly List<IMarkdownElement> _inlineElements;

        public MarkdownParser()
        {
            _inlineElements = new List<IMarkdownElement>
            {
                new BoldItalicElement(),
                new BoldElement(),
                new ItalicElement()
            };
        }

        public FlowDocument Parse(string markdownText)
        {
            if (string.IsNullOrEmpty(markdownText))
                return new FlowDocument();

            var document = new FlowDocument();
            var lines = markdownText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var blocks = ParseBlocks(lines);

            foreach (var block in blocks)
            {
                document.Blocks.Add(block);
            }

            return document;
        }

        private IEnumerable<Block> ParseBlocks(string[] lines)
        {
            var blocks = new List<Block>();
            int lineIndex = 0;

            while (lineIndex < lines.Length)
            {
                string line = lines[lineIndex];
                Block? block = null;

                if (TryParseHeader(line, out block))
                {
                    lineIndex++;
                }
                else if (TryParseList(lines, ref lineIndex, out block)) { }
                else if (TryParseParagraph(lines, ref lineIndex, out block)) { }
                else
                {
                    lineIndex++;
                }

                if (block != null)
                {
                    blocks.Add(block);
                }
            }

            return blocks;
        }

        private bool TryParseHeader(string line, out Block? block)
        {
            var headerMatch = Regex.Match(line, HeaderPattern);
            if (headerMatch.Success)
            {
                block = CreateHeaderBlock(headerMatch);
                return true;
            }

            block = null;
            return false;
        }

        private bool TryParseList(string[] lines, ref int lineIndex, out Block? block)
        {
            if (lines[lineIndex].TrimStart().StartsWith(ListSymbol))
            {
                var list = new List();

                while (lineIndex < lines.Length && lines[lineIndex].TrimStart().StartsWith(ListSymbol))
                {
                    list.ListItems.Add(CreateListItem(lines[lineIndex]));
                    lineIndex++;
                }

                block = list;
                return true;
            }

            block = null;
            return false;
        }

        private bool TryParseParagraph(string[] lines, ref int lineIndex, out Block? block)
        {
            var paragraphLines = new List<string>();

            while (lineIndex < lines.Length && IsSimpleText(lines[lineIndex]))
            {
                paragraphLines.Add(lines[lineIndex]);
                lineIndex++;
            }

            if (paragraphLines.Count > 0)
            {
                block = CreateParagraphBlock(paragraphLines);
                return true;
            }

            block = null;
            return false;
        }

        private Paragraph CreateHeaderBlock(Match headerMatch)
        {
            int headerLevel = headerMatch.Groups[1].Length;
            string headerText = headerMatch.Groups[2].Value;

            var header = new Paragraph
            {
                FontWeight = FontWeights.Bold,
                FontSize = HeaderBaseSize - (headerLevel * 2),
                Margin = new Thickness(0, 10, 0, 5)
            };

            foreach (var inline in ParseInlineMarkdown(headerText))
            {
                header.Inlines.Add(inline);
            }

            return header;
        }

        private ListItem CreateListItem(string line)
        {
            string listItemText = line.TrimStart().Substring(2);
            var paragraph = CreateParagraphBlock(new[] { listItemText });
            return new ListItem(paragraph);
        }

        private Paragraph CreateParagraphBlock(IEnumerable<string> lines)
        {
            var paragraph = new Paragraph();
            string paragraphText = string.Join(" ", lines);

            foreach (var inline in ParseInlineMarkdown(paragraphText))
            {
                paragraph.Inlines.Add(inline);
            }

            return paragraph;
        }

        private List<Inline> ParseInlineMarkdown(string text)
        {
            var inlines = new List<Inline>();
            int currentPosition = 0;

            while (currentPosition < text.Length)
            {
                bool matched = false;

                foreach (var element in _inlineElements)
                {
                    var inline = element.Parse(text, ref currentPosition);
                    if (inline != null)
                    {
                        inlines.Add(inline);
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    inlines.Add(new Run(text[currentPosition].ToString()));
                    currentPosition++;
                }
            }

            return inlines;
        }

        private bool IsSimpleText(string text)
        {
            return !string.IsNullOrWhiteSpace(text) &&
                   !Regex.IsMatch(text, @"^#{1,6}\s+") &&
                   !text.TrimStart().StartsWith(ListSymbol);
        }
    }
}