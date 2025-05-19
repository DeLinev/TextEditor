using System.Windows.Controls;

namespace TextEditor.Models.Commands
{
    public class ItalicTextCommand : InlineFormattingCommand
    {
        public ItalicTextCommand(TextBox editText, Document document)
            : base(editText, document, "*", "*")
        {
        }
    }
}
