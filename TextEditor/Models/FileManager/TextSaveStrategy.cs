using System.IO;
using System.Windows.Documents;

namespace TextEditor.Models.FileManager
{
    public class TextSaveStrategy : ISaveStrategy
    {
        public async Task<bool> Save(string filePath, string markdownContent, FlowDocument? flowDocument)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, markdownContent);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
