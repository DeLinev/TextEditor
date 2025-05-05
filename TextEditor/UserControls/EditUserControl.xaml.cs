using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TextEditor.Models;
using TextEditor.Models.Commands;
using TextEditor.Models.FileManager;
using TextEditor.Models.Parser;

namespace TextEditor.UserControls
{
    /// <summary>
    /// Interaction logic for EditUserControl.xaml
    /// </summary>
    public partial class EditUserControl : UserControl
    {
        public EditUserControl(Document? document)
        {
            InitializeComponent();
            if (document == null)
            {
                DisableUI();
            }

            CurrentDocument = document;
            UndoStack = new Stack<Command>();
            EditTextBox.Text = CurrentDocument?.Content ?? string.Empty;
        }

        private bool isExecutingCommand = false;
        public Document CurrentDocument { get; set; }
        public Stack<Command> UndoStack { get; set; }

        public event Action<UserControlTypes, Document> UserControlSwitched;
        public event Action<KeyValuePair<string, string>> FileSaved;

        public void ExecuteCommand(Command command)
        {
            isExecutingCommand = true;
            command.Execute();
            isExecutingCommand = false;
            UndoStack.Push(command);
            EditTextBox.Focus();
        }

        public void Undo()
        {
            if (UndoStack.Any())
            {
                var command = UndoStack.Pop();

                isExecutingCommand = true;
                command.Undo();
                isExecutingCommand = false;
            }
            EditTextBox.Focus();
        }

        private void PreviewBtn_Click(object sender, RoutedEventArgs e)
        {
            UserControlSwitched?.Invoke(UserControlTypes.Preview, CurrentDocument);
        }

        private void BoldBtn_Click(object sender, RoutedEventArgs e)
        {
            var boldCommand = new BoldTextCommand(EditTextBox, CurrentDocument);
            ExecuteCommand(boldCommand);
        }

        private void ItalicBtn_Click(object sender, RoutedEventArgs e)
        {
            var italicCommand = new ItalicTextCommand(EditTextBox, CurrentDocument);
            ExecuteCommand(italicCommand);
        }

        private void HeaderBtn_Click(object sender, RoutedEventArgs e)
        {
            var headerCommand = new InsertOnStartCommand(EditTextBox, CurrentDocument, '#');
            ExecuteCommand(headerCommand);
        }

        private void ListBtn_Click(object sender, RoutedEventArgs e)
        {
            var headerCommand = new InsertOnStartCommand(EditTextBox, CurrentDocument, '-');
            ExecuteCommand(headerCommand);
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            Undo();
        }

        private void EditTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isExecutingCommand)
                return;

            var editTextCommand = new EditTextCommand(EditTextBox, CurrentDocument);
            ExecuteCommand(editTextCommand);
        }

        private void EditTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Command command;

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.B)
            {
                command = new BoldTextCommand(EditTextBox, CurrentDocument);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.I)
            {
                command = new ItalicTextCommand(EditTextBox, CurrentDocument);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
            {
                e.Handled = true;
                Undo();
                return;
            }
            else
            {
                return;
            }

            ExecuteCommand(command);
        }

        private void DisableUI()
        {
            EditTextBox.IsEnabled = false;
            BoldBtn.IsEnabled = false;
            ItalicBtn.IsEnabled = false;
            HeaderBtn.IsEnabled = false;
            ListBtn.IsEnabled = false;
            UndoBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
            PreviewBtn.IsEnabled = false;
            SaveBtn.IsEnabled = false;
            SaveAsPdfBtn.IsEnabled = false;
            EditTextBox.Background = Brushes.LightGray;
            EditTextBox.Foreground = Brushes.Gray;
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileManager = new FileManager();
            var saveFileDialog = new SaveFileDialog 
            {
                Filter = FileManager.FileFilter,
                DefaultExt = ".md",
                FileName = CurrentDocument?.Path ?? "Untitled"
            };

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                bool isSaved = await fileManager.Save(filePath, EditTextBox.Text, null);
                if (isSaved)
                {
                    MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    var keyValuePair = new KeyValuePair<string, string>(saveFileDialog.SafeFileName, filePath);
                    FileSaved?.Invoke(keyValuePair);
                }
                else
                {
                    MessageBox.Show("Failed to save the file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SaveAsPdfBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentDocument != null)
            {
                var markdownParser = new MarkdownParser();
                FlowDocument doc = markdownParser.Parse(CurrentDocument.Content);

                var fileManager = new FileManager();
                fileManager.SetSaveStrategy(new PdfSaveStrategy());

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    DefaultExt = ".pdf",
                    FileName = "Untitled"
                };

                bool? result = saveFileDialog.ShowDialog();
                if (result == true)
                {
                    string filePath = saveFileDialog.FileName;
                    bool isSaved = await fileManager.Save(filePath, CurrentDocument.Content, doc);

                    if (isSaved)
                        MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Failed to save the file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
