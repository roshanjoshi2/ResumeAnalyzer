using ResumeAnalyzer.Models;

namespace ResumeAnalyzer.Services
{
    public interface IResumeAnalyzerService
    {
        Task<ResumeAnalysisResult> AnalyzeAsync(IFormFile resume, IFormFile jobDescription, string JobDescriptionText);

    }
}
