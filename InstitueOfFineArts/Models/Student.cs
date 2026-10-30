using InstituteOfFineArts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstituteOfFineArts.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ParentOrGuardianName { get; set; }

        [Phone]
        [StringLength(20)]
        public string? ParentOrGuardianPhone { get; set; }

        [Required]
        [StringLength(50)]
        public string EnrollmentNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime AdmissionDate { get; set; }

        [Required]
        [StringLength(150)]
        public string Course { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Class { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public ICollection<Award> Awards { get; set; } = new List<Award>();
    }
}