using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System.Windows.Documents;
using TextEditor.Models.FileManager.Helpers;
using Paragraph = System.Windows.Documents.Paragraph;
using Section = MigraDoc.DocumentObjectModel.Section;

namespace TextEditor.Models.FileManager
{
    public class PdfSaveStrategy : ISaveStrategy
    {
        private const string LeftIndent = "0.5cm";
        private const string BulletSymbol = "• ";

        public async Task<bool> Save(string filePath, string markdownContent, FlowDocument? flowDocument)
        {
            if (flowDocument == null)
                return false;

            try
            {
                var document = CreatePdfDocument(flowDocument);
                RenderAndSavePdf(document, filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private MigraDoc.DocumentObjectModel.Document CreatePdfDocument(FlowDocument flowDocument)
        {
            var document = new MigraDoc.DocumentObjectModel.Document();
            var section = document.AddSection();

            foreach (var block in flowDocument.Blocks)
            {
                if (block is Paragraph wpfParagraph)
                {
                    var paragraph = section.AddParagraph();
                    paragraph.Format.Font.Size = wpfParagraph.FontSize;
                    FlowDocToPdfParser.Parse(paragraph, wpfParagraph.Inlines);
                }
                else if (block is List wpfList)
                {
                    AddListToSection(section, wpfList);
                }
            }

            return document;
        }

        private void AddListToSection(Section section, List wpfList)
        {
            foreach (ListItem listItem in wpfList.ListItems)
            {
                foreach (var itemBlock in listItem.Blocks)
                {
                    if (itemBlock is Paragraph itemParagraph)
                    {
                        var paragraph = section.AddParagraph();
                        paragraph.Format.Font.Size = itemParagraph.FontSize;
                        paragraph.Format.LeftIndent = LeftIndent;
                        paragraph.AddFormattedText(BulletSymbol, TextFormat.Bold);
                        FlowDocToPdfParser.Parse(paragraph, itemParagraph.Inlines);
                    }
                }
            }
        }

        private void RenderAndSavePdf(MigraDoc.DocumentObjectModel.Document document, string filePath)
        {
            var renderer = new PdfDocumentRenderer
            {
                Document = document
            };
            renderer.RenderDocument();
            renderer.Save(filePath);
        }
    }
}
