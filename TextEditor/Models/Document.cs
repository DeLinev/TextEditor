using System.Text;

namespace TextEditor.Models
{
    public class Document
    {
        public string? Path { get; set; }
        public string Content => content.ToString();

        private StringBuilder content;

        public Document(string? path = null)
        {
            Path = path;
            content = new StringBuilder();
        }

        public void ChangeContent(string newContent)
        {
            content.Clear();
            content.Append(newContent);
        }

        public void InsertText(string text, int position)
        {
            content.Insert(position, text);
        }

        public void DeleteText(int position, int length)
        {
            content.Remove(position, length);
        }
    }
}
