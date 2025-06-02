using Microsoft.Win32;
using System.Windows.Documents;

namespace TextEditor.Models.FileManager
{
    public enum SaveFileType
    {
        Markdown,
        Pdf
    }

    public class FileSaveResult
    {
        public bool Success { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public static class FileSaveService
    {
        public static async Task<FileSaveResult> SaveFileAsync(
            SaveFileType fileType,
            string content,
            FlowDocument? flowDocument = null,
            string? defaultFileName = null)
        {
            var fileManager = new FileManager();
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            switch (fileType)
            {
                case SaveFileType.Markdown:
                    saveFileDialog.Filter = FileManager.FileFilter;
                    saveFileDialog.DefaultExt = ".md";
                    break;
                case SaveFileType.Pdf:
                    saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                    saveFileDialog.DefaultExt = ".pdf";
                    fileManager.SetSaveStrategy(new PdfSaveStrategy());
                    break;
            }

            saveFileDialog.FileName = defaultFileName ?? "Untitled";

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                bool isSaved = await fileManager.Save(filePath, content, flowDocument);
                if (isSaved)
                {
                    return new FileSaveResult
                    {
                        Success = true,
                        FilePath = filePath,
                        FileName = saveFileDialog.SafeFileName
                    };
                }
                else
                {
                    return new FileSaveResult
                    {
                        Success = false,
                        ErrorMessage = "Failed to save the file."
                    };
                }
            }
            return new FileSaveResult { Success = false };
        }
    }
}
