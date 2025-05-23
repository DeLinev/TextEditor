using System.Windows.Controls;

namespace TextEditor.Models.Commands
{
  public class BoldTextCommand : FormattedTextCommand
  {
    protected override string OpeningTag => "**";
    protected override string ClosingTag => "**";
    public BoldTextCommand(TextBox editText, Document document)
        : base(editText, document) { }
  }
}
