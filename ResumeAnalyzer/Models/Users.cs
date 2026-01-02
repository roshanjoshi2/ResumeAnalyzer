using Microsoft.AspNetCore.Identity;

namespace ResumeAnalyzer.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
    }
}
