using System.IO;
using System.Windows.Documents;

namespace TextEditor.Models.FileManager
{
    public class FileManager
    {
        private ISaveStrategy saveStrategy;
        public static string FileFilter { get; set; } = "Text Files (*.txt)|*.txt|Markdown Files (*.md)|*.md|All Files (*.*)|*.*";
        public FileManager()
        {
            saveStrategy = new TextSaveStrategy();
        }

        public void SetSaveStrategy(ISaveStrategy strategy)
        {
            saveStrategy = strategy;
        }

        public async Task<bool> Save(string filePath, string markdownContent, FlowDocument? flowDocument = null)
        {
            return await saveStrategy.Save(filePath, markdownContent, flowDocument);
        }

        public async Task<string> Open(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }
}
