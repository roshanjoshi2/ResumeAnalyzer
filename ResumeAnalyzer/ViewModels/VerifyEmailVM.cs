using System.ComponentModel.DataAnnotations;

namespace ResumeAnalyzer.ViewModels
{
    public class VerifyEmailVM
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
    }  

  
}
