using System.Windows.Controls;

namespace TextEditor.Models.Commands
{
    public class BoldTextCommand : Command
    {
        private int start, length;

        public BoldTextCommand(TextBox editText, Document document) : base(editText, document) 
        {
            start = _editTextBox.SelectionStart;
            length = _editTextBox.SelectionLength;
        }

        public override void Execute()
        {
            _document.ApplyBold(start, length);
            _editTextBox.Text = _document.Content;
            _editTextBox.CaretIndex = start + length + 2;        
        }

        public override void Undo()
        {
            _document.DeleteText(start + length + 2, 2);
            _document.DeleteText(start, 2);
            _editTextBox.Text = _document.Content;
            _editTextBox.CaretIndex = start;
        }
    }
}
