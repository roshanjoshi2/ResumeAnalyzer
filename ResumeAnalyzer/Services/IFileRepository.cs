namespace ResumeAnalyzer.Services
{
    public interface IFileRepository
    {
        Task<string> SaveFileAndExtractTextAsync(IFormFile file);
    }
}
