using InstituteOfFineArts.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InstituteOfFineArts.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Staff? Staff { get; set; }
        public Student? Student { get; set; }
    }
}
