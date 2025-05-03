using System.Windows.Controls;

namespace TextEditor.Models.Commands
{
    public class ItalicTextCommand : Command
    {
        private int start, length;

        public ItalicTextCommand(TextBox editText, Document document) : base(editText, document)
        {
            start = _editTextBox.SelectionStart;
            length = _editTextBox.SelectionLength;
        }

        public override void Execute()
        {
            _document.InsertText("*", start + length);
            _document.InsertText("*", start);
            _editTextBox.Text = _document.Content;
            _editTextBox.CaretIndex = start + length + 1;
        }

        public override void Undo()
        {
            _document.DeleteText(start + length + 1, 1);
            _document.DeleteText(start, 1);
            _editTextBox.Text = _document.Content;
            _editTextBox.CaretIndex = start;
        }
    }
}
