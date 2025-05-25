using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;

namespace TextEditor.Models.Parser
{
    public class BlockParser
    {
        private const string HeaderPattern = @"^(#{1,6})\s+(.+)$";
        private const string ListSymbol = "- ";
        private const int HeaderBaseSize = 26;

        private readonly InlineParser _inlineParser;

        public BlockParser(InlineParser inlineParser)
        {
            _inlineParser = inlineParser;
        }

        public IEnumerable<Block> ParseBlocks(string[] lines)
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

            foreach (var inline in _inlineParser.Parse(headerText))
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

            foreach (var inline in _inlineParser.Parse(paragraphText))
            {
                paragraph.Inlines.Add(inline);
            }

            return paragraph;
        }

        private bool IsSimpleText(string text)
        {
            return !string.IsNullOrWhiteSpace(text) &&
                   !Regex.IsMatch(text, @"^#{1,6}\s+") &&
                   !text.TrimStart().StartsWith(ListSymbol);
        }
    }
}
