using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeAnalyzer.Models
{
    public class JobSkill
    {
        [Key]
        public int JobSkillId { get; set; }

        [ForeignKey("JobDescription")]
        public int JobId { get; set; }
        public JobDescription Job { get; set; }

        [ForeignKey("EntityMaster")]
        public int EntityId { get; set; }
        public EntityMaster Entity { get; set; }

        public bool IsRequired { get; set; } = true;
        public float Weight { get; set; } = 1.0f;
    }
}
