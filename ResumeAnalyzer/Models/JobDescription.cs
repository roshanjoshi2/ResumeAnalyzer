using System.ComponentModel.DataAnnotations;

namespace ResumeAnalyzer.Models
{
    public class JobDescription
    {
        [Key]
        public int JobId { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string OriginalText { get; set; }
        public string CleanedText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<JobSkill> JobSkills { get; set; }
    }
}
