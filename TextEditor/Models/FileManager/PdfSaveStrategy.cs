using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System.Windows.Documents;

namespace TextEditor.Models.FileManager
{
    public class PdfSaveStrategy : ISaveStrategy
    {
        public async Task<bool> Save(string filePath, string markdownContent, FlowDocument? flowDocument)
        {
            if (flowDocument == null)
                return false;

            try
            {
                var doc = new MigraDoc.DocumentObjectModel.Document();
                var section = doc.AddSection();

                foreach (var block in flowDocument.Blocks)
                {
                    if (block is System.Windows.Documents.Paragraph wpfParagraph)
                    {
                        var migraParagraph = section.AddParagraph();
                        migraParagraph.Format.Font.Size = wpfParagraph.FontSize;
                        ParseInlineContent(migraParagraph, wpfParagraph.Inlines);
                    }
                    else if (block is List wpfList)
                    {
                        foreach (ListItem listItem in wpfList.ListItems)
                        {
                            foreach (var itemBlock in listItem.Blocks)
                            {
                                if (itemBlock is System.Windows.Documents.Paragraph itemPara)
                                {
                                    var para = section.AddParagraph();
                                    para.Format.LeftIndent = "0.5cm";

                                    string marker = "• ";
                                    para.AddFormattedText(marker, TextFormat.Bold);

                                    ParseInlineContent(para, itemPara.Inlines);
                                }
                            }
                        }
                    }
                }

                
                var renderer = new PdfDocumentRenderer
                {
                    Document = doc
                };
                renderer.RenderDocument();
                renderer.Save(filePath);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private void ParseInlineContent(MigraDoc.DocumentObjectModel.Paragraph migraParagraph, InlineCollection inlines)
        {
            foreach (var inline in inlines)
            {
                if (inline is Run run)
                {
                    var text = run.Text;
                    var format = migraParagraph.AddFormattedText(text);

                    if (run.FontWeight == System.Windows.FontWeights.Bold)
                        format.Bold = true;

                    if (run.FontStyle == System.Windows.FontStyles.Italic)
                        format.Italic = true;
                }
                else if (inline is Bold bold)
                {
                    foreach (var child in bold.Inlines)
                    {
                        if (child is Run boldRun)
                        {
                            var format = migraParagraph.AddFormattedText(boldRun.Text);
                            format.Bold = true;
                        }
                    }
                }
                else if (inline is Italic italic)
                {
                    foreach (var child in italic.Inlines)
                    {
                        if (child is Run italicRun)
                        {
                            var format = migraParagraph.AddFormattedText(italicRun.Text);
                            format.Italic = true;
                        }
                    }
                }
            }
        }

    }
}
