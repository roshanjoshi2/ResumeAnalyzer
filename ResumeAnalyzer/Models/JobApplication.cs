using System.ComponentModel.DataAnnotations;

namespace ResumeAnalyzer.Models
{
    public class JobApplication:IValidatableObject
    {
        [Required]
        [Display(Name = "Resume")]
        public IFormFile Resume { get; set; }

      
        [Display(Name = "Or Choose Job Description File")]        
        public IFormFile JobDescription { get; set; }

        [Display(Name = "Job Description Text")]
        public string JobDescriptionText { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (JobDescription == null && string.IsNullOrWhiteSpace(JobDescriptionText))
            {
                yield return new ValidationResult(
                    "Please provide a job description (file or text).",
                    new[] { nameof(JobDescription), nameof(JobDescriptionText) });
            }
        }
    }
}
