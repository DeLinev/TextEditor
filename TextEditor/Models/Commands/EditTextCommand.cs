using System.Windows.Controls;

namespace TextEditor.Models.Commands
{
    public class EditTextCommand : Command
    {
        private string backupText;
        private int caretIndex;

        public EditTextCommand(TextBox textBox, Document document) : base(textBox, document)
        {
            backupText = document.Content;
        }

        public override void Execute()
        {
            backupText = _document.Content;
            caretIndex = _editTextBox.CaretIndex;
            _document.ChangeContent(_editTextBox.Text);
        }

        public override void Undo()
        {
            _document.ChangeContent(backupText);
            _editTextBox.Text = _document.Content;
            if (caretIndex >= 0 && caretIndex <= _editTextBox.Text.Length)
                _editTextBox.CaretIndex = caretIndex;
            else
                _editTextBox.CaretIndex = _editTextBox.Text.Length;
        }
    }
}
