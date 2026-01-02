using System.ComponentModel.DataAnnotations;

namespace ResumeAnalyzer.Models
{
    public class ResumeParsedData
    {
        [Key]
        public int ResumeId { get; set; }
        public int? UserId { get; set; } // optional
        public string OriginalText { get; set; }
        public string CleanedText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<ResumeSkill> ResumeSkills { get; set; }
    }
}
