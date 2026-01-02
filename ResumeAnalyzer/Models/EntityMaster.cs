using System.ComponentModel.DataAnnotations;

namespace ResumeAnalyzer.Models
{
    public class EntityMaster
    {
        [Key]
        public int EntityId { get; set; }
        public string EntityType { get; set; } // Skill, Degree, Tool
        public string EntityName { get; set; }
        public string NormalizedName { get; set; }
        public string Synonyms { get; set; }
        public string Domain { get; set; }
        public string SubDomain { get; set; }
    }
}
