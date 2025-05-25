using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using TextEditor.Models.Parser.Interpreter;

namespace TextEditor.Models.Parser
{
    public class InlineParser
    {
        private readonly List<IMarkdownElement> _inlineElements;

        public InlineParser()
        {
            _inlineElements = new List<IMarkdownElement>
            {
                new BoldItalicElement(),
                new BoldElement(),
                new ItalicElement()
            };
        }

        public List<Inline> Parse(string text)
        {
            var inlines = new List<Inline>();
            int currentPosition = 0;

            while (currentPosition < text.Length)
            {
                bool matched = false;

                foreach (var element in _inlineElements)
                {
                    var inline = element.Parse(text, ref currentPosition);
                    if (inline != null)
                    {
                        inlines.Add(inline);
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    inlines.Add(new Run(text[currentPosition].ToString()));
                    currentPosition++;
                }
            }

            return inlines;
        }
    }
}
