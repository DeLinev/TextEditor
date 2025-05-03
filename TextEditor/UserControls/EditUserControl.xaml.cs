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
        public EditUserControl(string? path = null)
        {
            InitializeComponent();
            UndoStack = new Stack<Command>();
            CurrentDocument = new Document(path);
        }

        public Document CurrentDocument { get; set; }
        public Stack<Command> UndoStack { get; set; }

        public event Action<UserControlTypes> UserControlSwitched;

        public void ExecuteCommand(Command command)
        {
            command.Execute();
            UndoStack.Push(command);
        }

        public void Undo()
        {
            if (UndoStack.Any())
            {
                var command = UndoStack.Pop();
                command.Undo();
            }
        }

        private void PreviewBtn_Click(object sender, RoutedEventArgs e)
        {
            UserControlSwitched?.Invoke(UserControlTypes.Preview);
        }

        private void BoldBtn_Click(object sender, RoutedEventArgs e)
        {
            var boldCommand = new BoldTextCommand(EditTextBox, CurrentDocument);
            ExecuteCommand(boldCommand);
        }

        private void EditTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentDocument.ChangeContent(EditTextBox.Text);
        }

        private void EditTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.B)
            {
                var boldCommand = new BoldTextCommand(EditTextBox, CurrentDocument);
                ExecuteCommand(boldCommand);
            }
        }
    }
}
