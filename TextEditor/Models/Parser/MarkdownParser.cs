using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;

namespace TextEditor.Models.Parser
{
    public class MarkdownParser
    {
        private readonly Dictionary<int, Style> _headerStyles = new Dictionary<int, Style>();

        public MarkdownParser()
        {
            InitializeStyles();
        }

        private void InitializeStyles()
        {
            for (int i = 1; i <= 6; i++)
            {
                _headerStyles[i] = new Style(typeof(Paragraph))
                {
                    Setters = {
                        new Setter(Paragraph.FontWeightProperty, FontWeights.Bold),
                        new Setter(Paragraph.FontSizeProperty, 26 - (i * 2)),
                        new Setter(Paragraph.MarginProperty, new Thickness(0, 10, 0, 5))
                    }
                };
            }
        }

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

                    block = header;
                    lineIndex++;
                }
                else if (line.TrimStart().StartsWith("- "))
                {
                    List list = new List() { MarkerStyle = TextMarkerStyle.Disc, Margin = new Thickness(5, 5, 0, 5) };

                    while (lineIndex < lines.Length && lines[lineIndex].TrimStart().StartsWith("- "))
                    {
                        string listItemText = lines[lineIndex].TrimStart().Substring(2);
                        var paragraph = new Paragraph();
                        foreach (var inline in ParseInlineMarkdown(listItemText))
                        {
                            paragraph.Inlines.Add(inline);
                        }
                        ListItem item = new ListItem(paragraph);
                        list.ListItems.Add(item);
                        lineIndex++;
                    }

                    block = list;
                }
                else
                {
                    List<string> paragraphLines = new List<string>();

                    while (lineIndex < lines.Length && !string.IsNullOrWhiteSpace(lines[lineIndex]) &&
                          !Regex.IsMatch(lines[lineIndex], @"^#{1,6}\s+") &&
                          !lines[lineIndex].TrimStart().StartsWith("- "))
                    {
                        paragraphLines.Add(lines[lineIndex]);
                        lineIndex++;
                    }

                    if (paragraphLines.Count > 0)
                    {
                        string paragraphText = string.Join(" ", paragraphLines);
                        var par = new Paragraph();
                        foreach (var inline in ParseInlineMarkdown(paragraphText))
                        {
                            par.Inlines.Add(inline);
                        }
                        block = par;
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
    }
}
