using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TailorrNow.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            FullName = string.Empty;
        }

        [Required]
        public string FullName { get; set; }
    }
}
