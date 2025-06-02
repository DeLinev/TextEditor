using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TextEditor.Models;
using TextEditor.Models.FileManager;
using TextEditor.Models.Parser;

namespace TextEditor.UserControls
{
    /// <summary>
    /// Interaction logic for PreviewUserControl.xaml
    /// </summary>
    public partial class PreviewUserControl : UserControl
    {
        public PreviewUserControl(Document document)
        {
            InitializeComponent();
            currentDocument = document;
            markdownParser = new MarkdownParser();

            UpdatePreview();
        }

        private MarkdownParser markdownParser;
        private Document currentDocument;

        public event Action<UserControlTypes, Document> UserControlSwitched;
        public event Action<KeyValuePair<string, string>> FileSaved;

        private void UpdatePreview()
        {
            if (currentDocument != null)
            {
                FlowDocument doc = markdownParser.Parse(currentDocument.Content);
                doc.TextAlignment = TextAlignment.Left;
                PreviewRichTextBox.Document = doc;
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            UserControlSwitched?.Invoke(UserControlTypes.Edit, currentDocument);
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await FileSaveService.SaveFileAsync(
                SaveFileType.Markdown,
                currentDocument.Content,
                null,
                currentDocument?.Path ?? "Untitled"
            );

            if (result.Success)
            {
                MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                if (result.FileName != null && result.FilePath != null)
                {
                    var keyValuePair = new KeyValuePair<string, string>(result.FileName, result.FilePath);
                    FileSaved?.Invoke(keyValuePair);
                }
                else if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    MessageBox.Show(result.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SaveAsPdfBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentDocument != null)
            {
                FlowDocument doc = markdownParser.Parse(currentDocument.Content);
                var result = await FileSaveService.SaveFileAsync(
                    SaveFileType.Pdf,
                    currentDocument.Content,
                    doc,
                    "Untitled"
                );

                if (result.Success)
                {
                    MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    MessageBox.Show(result.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
