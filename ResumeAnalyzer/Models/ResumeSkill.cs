using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeAnalyzer.Models
{
    public class ResumeSkill
    {
        [Key]
        public int ResumeSkillId { get; set; }

        [ForeignKey("ResumeParsedData")]
        public int ResumeId { get; set; }
        public ResumeParsedData Resume { get; set; }

        [ForeignKey("EntityMaster")]
        public int EntityId { get; set; }
        public EntityMaster Entity { get; set; }

        public float ConfidenceScore { get; set; } = 1.0f;
    }
}
