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

        private void UpdatePreview()
        {
            if (currentDocument != null)
            {
                FlowDocument doc = markdownParser.Parse(currentDocument.Content);
                PreviewRichTextBox.Document = doc;
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            UserControlSwitched?.Invoke(UserControlTypes.Edit, currentDocument);
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileManager = new FileManager();
            var saveFileDialog = new SaveFileDialog
            {
                Filter = fileManager.FileFilter,
                DefaultExt = ".md",
            };

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                bool isSaved = await fileManager.Save(filePath, currentDocument.Content, null);
                if (isSaved)
                {
                    MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to save the file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
