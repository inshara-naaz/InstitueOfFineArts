using InstituteOfFineArts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstituteOfFineArts.Models
{
    public class Competition
    {
        [Key]
        public int CompetitionId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [StringLength(3000)]
        public string? Conditions { get; set; }

        public CompetitionStatus Status { get; set; } = CompetitionStatus.Upcoming;

        [Required]
        public int StaffId { get; set; }

        [ForeignKey(nameof(StaffId))]
        public Staff ManagedByStaff { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string? ImagePath { get; set; }

        public ICollection<CompetitionAward> CompetitionAwards { get; set; } = new List<CompetitionAward>();
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public ICollection<Award> IssuedAwards { get; set; } = new List<Award>();
    }

    public enum CompetitionStatus
    {
        Upcoming = 1,
        Ongoing = 2,
        Concluded = 3
    }
}