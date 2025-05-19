using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Models.Commands
{
    public abstract class InlineFormattingCommand : Command
    {
        protected readonly string _openTag;
        protected readonly string _closeTag;
        protected int _start;
        protected int _length;

        public InlineFormattingCommand(TextBox editText, Document document, string openTag, string closeTag)
            : base(editText, document)
        {
            _openTag = openTag;
            _closeTag = closeTag;
            _start = _editTextBox.SelectionStart;
            _length = _editTextBox.SelectionLength;
        }

        public override void Execute()
        {
            _document.InsertText(_closeTag, _start + _length);
            _document.InsertText(_openTag, _start);
            UpdateTextBox(_start + _length + _openTag.Length + _closeTag.Length);
        }

        public override void Undo()
        {
            _document.DeleteText(_start + _length + _closeTag.Length, _closeTag.Length);
            _document.DeleteText(_start, _openTag.Length);
            UpdateTextBox(_start);
        }

        protected void UpdateTextBox(int caretPosition)
        {
            _editTextBox.Text = _document.Content;
            _editTextBox.CaretIndex = caretPosition;
        }
    }
}
