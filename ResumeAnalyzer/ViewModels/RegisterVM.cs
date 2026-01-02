using System.ComponentModel.DataAnnotations;

namespace ResumeAnalyzer.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage ="Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email  { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(40,MinimumLength =8,ErrorMessage ="The{0}must be at {2} and at max {1} character")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword",ErrorMessage ="Password and Confirm Password do not match")]
        [Display(Name = " Password")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]

        public string ConfirmPassword { get; set; }
    }
}
