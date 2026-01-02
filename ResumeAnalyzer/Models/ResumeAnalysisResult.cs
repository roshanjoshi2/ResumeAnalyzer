namespace ResumeAnalyzer.Models
{
    public class ResumeAnalysisResult
    {
        public double MatchPercentage { get; set; }
        public string ResumeText { get; set; }
        public string JobDescriptionText { get; set; }
    }
}
