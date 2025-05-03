using System.Windows.Controls;

namespace TextEditor.Models.Commands
{
    public class InsertOnStartCommand : Command
    {
        private int insertPos, charsInserted;
        private char symbol;

        public InsertOnStartCommand(TextBox editText, Document document, char symbol) : base(editText, document) 
        {
            this.symbol = symbol;
        }

        public override void Execute()
        {
            int currentCharPos = GetLineStart();
            insertPos = currentCharPos;

            if (InsertOneOrTwoSymbols(currentCharPos))
            {
                _document.InsertText(symbol.ToString(), currentCharPos);
                charsInserted = 1;
            }
            else
            {
                _document.InsertText(symbol.ToString() + " ", currentCharPos);
                charsInserted = 2;
            }

            _editTextBox.Text = _document.Content;
            _editTextBox.CaretIndex = _document.Content.Length;
        }


        public override void Undo()
        {
            _document.DeleteText(insertPos, charsInserted);
            _editTextBox.Text = _document.Content;
            _editTextBox.CaretIndex = _document.Content.Length;
        }

        private bool InsertOneOrTwoSymbols(int currentCharPos)
        {
            return !string.IsNullOrEmpty(_document.Content) 
                && currentCharPos < _document.Content.Length 
                && _document.Content[currentCharPos] == symbol;
        }

        private int GetLineStart()
        {
            int currentCharPos = _editTextBox.CaretIndex;

            while (currentCharPos > 0)
            {
                if (_document.Content[currentCharPos - 1] == '\n')
                {
                    break;
                }
                currentCharPos--;
            }
            return currentCharPos;
        }
    }
}
