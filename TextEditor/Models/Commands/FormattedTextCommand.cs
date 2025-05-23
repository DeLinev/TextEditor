using System.Windows.Controls;

namespace TextEditor.Models.Commands
{
  public abstract class FormattedTextCommand : Command
  {
    protected int start, length;

    protected abstract string OpeningTag { get; }
    protected abstract string ClosingTag { get; }

    public FormattedTextCommand(TextBox editText, Document document)
        : base(editText, document)
    {
      start = _editTextBox.SelectionStart;
      length = _editTextBox.SelectionLength;
    }

    public override void Execute()
    {
      InsertTags();
      UpdateEditor();
    }

    public override void Undo()
    {
      RemoveTags();
      UpdateEditor();
    }

    private void InsertTags()
    {
      _document.InsertText(ClosingTag, start + length);
      _document.InsertText(OpeningTag, start);
    }

    private void RemoveTags()
    {
      _document.DeleteText(start + length + ClosingTag.Length, ClosingTag.Length);
      _document.DeleteText(start, OpeningTag.Length);
    }

    private void UpdateEditor()
    {
      _editTextBox.Text = _document.Content;
      _editTextBox.CaretIndex = start + length + OpeningTag.Length + ClosingTag.Length;
    }
  }
}
