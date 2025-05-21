using System.Windows.Controls;

namespace TextEditor.Models.Commands
{
  public class ItalicTextCommand : FormattedTextCommand
  {
    protected override string OpeningTag => "*";
    protected override string ClosingTag => "*";
    public ItalicTextCommand(TextBox editText, Document document)
        : base(editText, document) { }
  }
}
