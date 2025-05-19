using System.Windows.Controls;

namespace TextEditor.Models.Commands
{
    public class BoldTextCommand : InlineFormattingCommand
    {
        public BoldTextCommand(TextBox editText, Document document)
            : base(editText, document, "**", "**")
        {
        }
    }
}
