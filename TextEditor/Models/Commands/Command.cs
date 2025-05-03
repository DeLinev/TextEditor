using System.Windows.Controls;

namespace TextEditor.Models.Commands
{
    public abstract class Command
    {
        protected TextBox _editTextBox;
        protected Document _document;

        public Command(TextBox editText, Document document) 
        {
            _editTextBox = editText;
            _document = document;
        }

        abstract public void Execute();
        abstract public void Undo();
    }
}
