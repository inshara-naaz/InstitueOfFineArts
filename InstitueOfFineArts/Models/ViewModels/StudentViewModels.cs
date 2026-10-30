using System.ComponentModel.DataAnnotations;

namespace InstituteOfFineArts.Models.ViewModels
{
    public class StudentCreateViewModel
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
        [Display(Name = "Parent / Guardian Name")]
        public string? ParentOrGuardianName { get; set; }

        [Phone]
        [StringLength(20)]
        [Display(Name = "Parent / Guardian Phone")]
        public string? ParentOrGuardianPhone { get; set; }

        // ── Admission Details ──────────────────────────────────────────────────
        [Required]
        [StringLength(50)]
        [Display(Name = "Enrollment Number")]
        public string EnrollmentNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Admission Date")]
        public DateTime AdmissionDate { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Course")]
        public string Course { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Class")]
        public string? Class { get; set; }

        [StringLength(500)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }
    }

    public class StudentEditViewModel
    {
        public int StudentId { get; set; }

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
        [Display(Name = "Parent / Guardian Name")]
        public string? ParentOrGuardianName { get; set; }

        [Phone]
        [StringLength(20)]
        [Display(Name = "Parent / Guardian Phone")]
        public string? ParentOrGuardianPhone { get; set; }

        // ── Admission Details ──────────────────────────────────────────────────
        [Required]
        [StringLength(50)]
        [Display(Name = "Enrollment Number")]
        public string EnrollmentNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Admission Date")]
        public DateTime AdmissionDate { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Course")]
        public string Course { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Class")]
        public string? Class { get; set; }

        [StringLength(500)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        public bool IsActive { get; set; }
    }

    public class StudentDetailsViewModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ParentOrGuardianName { get; set; }
        public string? ParentOrGuardianPhone { get; set; }
        public string EnrollmentNumber { get; set; } = string.Empty;
        public DateTime AdmissionDate { get; set; }
        public string Course { get; set; } = string.Empty;
        public string? Class { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}
