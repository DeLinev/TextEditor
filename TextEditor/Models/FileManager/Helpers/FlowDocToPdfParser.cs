using System.Windows.Documents;

namespace TextEditor.Models.FileManager.Helpers
{
    public static class FlowDocToPdfParser
    {
        public static void Parse(MigraDoc.DocumentObjectModel.Paragraph paragraph, InlineCollection inlines)
        {
            foreach (var inline in inlines)
            {
                if (inline is Run run)
                {
                    AddRunToParagraph(paragraph, run);
                }
                else if (inline is Bold bold)
                {
                    AddFormattedInlinesToParagraph(paragraph, bold.Inlines, isBold: true);
                }
                else if (inline is Italic italic)
                {
                    AddFormattedInlinesToParagraph(paragraph, italic.Inlines, isItalic: true);
                }
            }
        }

        private static void AddRunToParagraph(MigraDoc.DocumentObjectModel.Paragraph paragraph, Run run)
        {
            var text = run.Text;
            var format = paragraph.AddFormattedText(text);

            if (run.FontWeight == System.Windows.FontWeights.Bold)
                format.Bold = true;

            if (run.FontStyle == System.Windows.FontStyles.Italic)
                format.Italic = true;
        }

        private static void AddFormattedInlinesToParagraph(MigraDoc.DocumentObjectModel.Paragraph paragraph, InlineCollection inlines, bool isBold = false, bool isItalic = false)
        {
            foreach (var inline in inlines)
            {
                if (inline is Run run)
                {
                    var format = paragraph.AddFormattedText(run.Text);
                    format.Bold = isBold;
                    format.Italic = isItalic;
                }
            }
        }
    }
}
