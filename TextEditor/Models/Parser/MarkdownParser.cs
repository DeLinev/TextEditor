using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;

namespace TextEditor.Models.Parser
{
    public class MarkdownParser
    {
        public FlowDocument Parse(string markdownText)
        {
            FlowDocument document = new FlowDocument();

            if (string.IsNullOrEmpty(markdownText))
                return document;

            string[] lines = markdownText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            int lineIndex = 0;
            List<Block> blocks = new List<Block>();

            while (lineIndex < lines.Length)
            {
                string line = lines[lineIndex];
                Block? block = null;

                Match headerMatch = Regex.Match(line, @"^(#{1,6})\s+(.+)$");
                if (headerMatch.Success)
                {
                    block = GetHeader(headerMatch);
                    lineIndex++;
                }
                else if (line.TrimStart().StartsWith("- "))
                {
                    List list = new List();

                    while (lineIndex < lines.Length && lines[lineIndex].TrimStart().StartsWith("- "))
                    {
                        ListItem item = GetListItem(lines[lineIndex]);
                        list.ListItems.Add(item);
                        lineIndex++;
                    }

                    block = list;
                }
                else
                {
                    List<string> paragraphLines = new List<string>();

                    while (lineIndex < lines.Length && IsSimpleText(lines[lineIndex]))
                    {
                        paragraphLines.Add(lines[lineIndex]);
                        lineIndex++;
                    }

                    if (paragraphLines.Count > 0)
                    {
                        string paragraphText = string.Join(" ", paragraphLines);
                        var paragraph = new Paragraph();
                        foreach (var inline in ParseInlineMarkdown(paragraphText))
                        {
                            paragraph.Inlines.Add(inline);
                        }
                        block = paragraph;
                    }
                    else
                    {
                        lineIndex++;
                    }
                }

                if (block != null)
                {
                    blocks.Add(block);
                }
            }

            foreach (var block in blocks)
            {
                document.Blocks.Add(block);
            }

            return document;
        }

        private List<Inline> ParseInlineMarkdown(string text)
        {
            List<Inline> inlines = new List<Inline>();

            int currentPosition = 0;

            while (currentPosition < text.Length)
            {
                Match boldMatch = Regex.Match(text.Substring(currentPosition), @"^\*\*(.+?)\*\*");
                if (boldMatch.Success)
                {
                    if (boldMatch.Index > 0)
                    {
                        inlines.Add(new Run(text.Substring(currentPosition, boldMatch.Index)));
                    }
                    string boldText = boldMatch.Groups[1].Value;
                    inlines.Add(new Bold(new Run(boldText)));
                    currentPosition += boldMatch.Index + boldMatch.Length;
                    continue;
                }

                Match italicMatch = Regex.Match(text.Substring(currentPosition), @"^\*(.+?)\*");
                if (italicMatch.Success)
                {
                    if (italicMatch.Index > 0)
                    {
                        inlines.Add(new Run(text.Substring(currentPosition, italicMatch.Index)));
                    }
                    string italicText = italicMatch.Groups[1].Value;
                    inlines.Add(new Italic(new Run(italicText)));
                    currentPosition += italicMatch.Index + italicMatch.Length;
                    continue;
                }

                int nextSpecialChar = FindNextSpecialChar(text, currentPosition);
                if (nextSpecialChar == -1)
                {
                    inlines.Add(new Run(text.Substring(currentPosition)));
                    break;
                }
                else if (nextSpecialChar > currentPosition)
                {
                    inlines.Add(new Run(text.Substring(currentPosition, nextSpecialChar - currentPosition)));
                    currentPosition = nextSpecialChar;
                }
                else
                {
                    inlines.Add(new Run(text[currentPosition].ToString()));
                    currentPosition++;
                }
            }

            return inlines;
        }

        private int FindNextSpecialChar(string text, int startIndex)
        {
            int[] positions = new[]
            {
                text.IndexOf("**", startIndex),
                text.IndexOf('*', startIndex),
            };

            return positions.Where(p => p >= 0).DefaultIfEmpty(-1).Min();
        }

        private Block GetHeader(Match headerMatch)
        {
            int headerLevel = headerMatch.Groups[1].Length;
            string headerText = headerMatch.Groups[2].Value;

            Paragraph header = new Paragraph();
            foreach (var inline in ParseInlineMarkdown(headerText))
            {
                header.Inlines.Add(inline);
            }

            header.FontWeight = FontWeights.Bold;
            header.FontSize = 26 - (headerLevel * 2);
            header.Margin = new Thickness(0, 10, 0, 5);

            return header;
        }

        private ListItem GetListItem(string line)
        {
            string listItemText = line.TrimStart().Substring(2);
            var paragraph = new Paragraph();
            foreach (var inline in ParseInlineMarkdown(listItemText))
            {
                paragraph.Inlines.Add(inline);
            }
            return new ListItem(paragraph);
        }

        private bool IsSimpleText(string text)
        {
            return !string.IsNullOrWhiteSpace(text) 
                && !Regex.IsMatch(text, @"^#{1,6}\s+") 
                && !text.TrimStart().StartsWith("- ");
        }
    }
}
