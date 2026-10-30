using System.ComponentModel.DataAnnotations;

namespace InstituteOfFineArts.Models.ViewModels
{
    public class StaffCreateViewModel
    {
        // ── Login Details ──────────────────────────────────────────────────────
        [Required]
        [EmailAddress]
        [Display(Name = "Email (Login)")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // ── Personal Details ───────────────────────────────────────────────────
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Emergency Contact")]
        public string? EmergencyContact { get; set; }

        [StringLength(200)]
        [Display(Name = "Qualification")]
        public string? Qualification { get; set; }

        // ── Work Details ───────────────────────────────────────────────────────
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; }

        [StringLength(300)]
        [Display(Name = "Handles Class")]
        public string? HandlesClass { get; set; }

        [StringLength(300)]
        [Display(Name = "Handles Subject")]
        public string? HandlesSubject { get; set; }

        [StringLength(100)]
        [Display(Name = "Designation")]
        public string? Designation { get; set; }

        // ── Remarks ────────────────────────────────────────────────────────────
        [StringLength(1000)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }
    }

    public class StaffEditViewModel
    {
        public int StaffId { get; set; }

        // ── Personal Details ───────────────────────────────────────────────────
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Emergency Contact")]
        public string? EmergencyContact { get; set; }

        [StringLength(200)]
        [Display(Name = "Qualification")]
        public string? Qualification { get; set; }

        // ── Work Details ───────────────────────────────────────────────────────
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; }

        [StringLength(300)]
        [Display(Name = "Handles Class")]
        public string? HandlesClass { get; set; }

        [StringLength(300)]
        [Display(Name = "Handles Subject")]
        public string? HandlesSubject { get; set; }

        [StringLength(100)]
        [Display(Name = "Designation")]
        public string? Designation { get; set; }

        [StringLength(1000)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        public bool IsActive { get; set; }
    }

    public class StaffDetailsViewModel
    {
        public int StaffId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? EmergencyContact { get; set; }
        public string? Qualification { get; set; }
        public DateTime DateJoined { get; set; }
        public string? HandlesClass { get; set; }
        public string? HandlesSubject { get; set; }
        public string? Designation { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}