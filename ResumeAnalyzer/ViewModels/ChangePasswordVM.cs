using System.ComponentModel.DataAnnotations;

namespace ResumeAnalyzer.ViewModels
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "New Password is required")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The{0}must be at {2} and at max {1} character")]
        [DataType(DataType.Password)]
        [Compare("ConfirmNewPassword", ErrorMessage = "Password and Confirm Password do not match")]
        [Display(Name = "New Password")]

        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm New Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]


        public string ConfirmNewPassword { get; set; }
      
    }
}
