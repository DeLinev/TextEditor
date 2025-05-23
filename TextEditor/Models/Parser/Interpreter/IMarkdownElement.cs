using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace TextEditor.Models.Parser.Interpreter
{
    public interface IMarkdownElement
    {
        Inline? Parse(string text, ref int currentPosition);
    }
}
