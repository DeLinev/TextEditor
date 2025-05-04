using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TextEditor.Models;
using TextEditor.Models.Commands;

namespace TextEditor.UserControls
{
    /// <summary>
    /// Interaction logic for EditUserControl.xaml
    /// </summary>
    public partial class EditUserControl : UserControl
    {
        public EditUserControl(Document document)
        {
            InitializeComponent();
            CurrentDocument = document;
            UndoStack = new Stack<Command>();
        }

        private bool isExecutingCommand = false;
        public Document CurrentDocument { get; set; }
        public Stack<Command> UndoStack { get; set; }

        public event Action<UserControlTypes, Document> UserControlSwitched;

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

    }
}
