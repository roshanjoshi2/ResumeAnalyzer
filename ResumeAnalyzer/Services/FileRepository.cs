using DocumentFormat.OpenXml.Packaging;
using System.Text;





namespace ResumeAnalyzer.Services
{
    public class FileRepository : IFileRepository
    {
        private readonly IWebHostEnvironment _env;

        public FileRepository(IWebHostEnvironment env)
        {
            _env = env;
        }
        public string ReadWordFile(string filePath)
        {
            var sb = new StringBuilder();

            // Open the Word document for read-only access
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                // Get the main document part
                var body = wordDoc.MainDocumentPart.Document.Body;

                // Append the text from the document body
                sb.AppendLine(body.InnerText);
            }

            return sb.ToString();
        }
        
        public string pdfExtractText(string filePath)
        {
            var sb = new StringBuilder();

            using (var document = PdfiumViewer.PdfDocument.Load(filePath))
            {
                for (int i = 0; i < document.PageCount; i++)
                {
                    sb.AppendLine(document.GetPdfText(i));
                }
            }

            return sb.ToString();
        }



        public async Task<string> SaveFileAndExtractTextAsync(IFormFile file)
        {
            string uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            string filePath = Path.Combine(uploadFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Simple text extraction for .txt
            if (Path.GetExtension(file.FileName).ToLower() == ".txt")
            {
                return await File.ReadAllTextAsync(filePath);
            }

            else if (Path.GetExtension(file.FileName).ToLower() == ".docx"){
                var result = ReadWordFile(filePath);
                return result;

            }
            else if (Path.GetExtension(file.FileName).ToLower() == ".pdf")
            {
                var result = pdfExtractText(filePath);

                return result;
            }
            else
            {
                throw new NotSupportedException("File type not supported. Only TXT, DOCX, and PDF are allowed.");

            }

            return $"File saved: {file.FileName}";
        }
    }

}
