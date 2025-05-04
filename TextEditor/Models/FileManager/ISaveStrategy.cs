using System.Windows.Documents;

namespace TextEditor.Models.FileManager
{
    public interface ISaveStrategy
    {
        Task<bool> Save(string filePath, string markdownContent, FlowDocument? flowDocument);
    }
}
