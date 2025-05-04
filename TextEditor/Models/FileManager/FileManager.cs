using System.IO;
using System.Windows.Documents;

namespace TextEditor.Models.FileManager
{
    public class FileManager
    {
        public string FileFilter { get; set; }
        private ISaveStrategy saveStrategy;

        public FileManager(string fileFilter = "Text Files (*.txt)|*.txt|Markdown Files (*.md)|*.md")
        {
            saveStrategy = new TextSaveStrategy();
            FileFilter = fileFilter;
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
