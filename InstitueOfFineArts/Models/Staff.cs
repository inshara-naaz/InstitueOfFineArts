using InstituteOfFineArts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstituteOfFineArts.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

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
        public string? EmergencyContact { get; set; }

        [StringLength(200)]
        public string? Qualification { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateJoined { get; set; }

        [StringLength(300)]
        public string? HandlesClass { get; set; }

        [StringLength(300)]
        public string? HandlesSubject { get; set; }

        [StringLength(100)]
        public string? Designation { get; set; }

        [StringLength(1000)]
        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Competition> ManagedCompetitions { get; set; } = new List<Competition>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
        public ICollection<Award> IssuedAwards { get; set; } = new List<Award>();
        public ICollection<Exhibition> ManagedExhibitions { get; set; } = new List<Exhibition>();
        public ICollection<ExhibitionListing> ExhibitionListings { get; set; } = new List<ExhibitionListing>();
    }
}