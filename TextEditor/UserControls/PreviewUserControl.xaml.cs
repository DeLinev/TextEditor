using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TextEditor.Models;
using TextEditor.Models.Parser;

namespace TextEditor.UserControls
{
    /// <summary>
    /// Interaction logic for PreviewUserControl.xaml
    /// </summary>
    public partial class PreviewUserControl : UserControl
    {
        public PreviewUserControl(Models.Document document)
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
    }
}
