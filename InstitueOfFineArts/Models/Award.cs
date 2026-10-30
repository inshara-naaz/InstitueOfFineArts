using InstituteOfFineArts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstituteOfFineArts.Models
{
    public class Award
    {
        [Key]
        public int AwardId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        [Required]
        public int CompetitionId { get; set; }

        [ForeignKey(nameof(CompetitionId))]
        public Competition Competition { get; set; } = null!;

        public int? CompetitionAwardId { get; set; }

        [ForeignKey(nameof(CompetitionAwardId))]
        public CompetitionAward? CompetitionAward { get; set; }

        [Required]
        public int SubmissionId { get; set; }

        [ForeignKey(nameof(SubmissionId))]
        public Submission WinningSubmission { get; set; } = null!;

        [Required]
        public int IssuedByStaffId { get; set; }

        [ForeignKey(nameof(IssuedByStaffId))]
        public Staff IssuedByStaff { get; set; } = null!;

        [StringLength(200)]
        public string? CustomAwardTitle { get; set; }

        [StringLength(2000)]
        public string? Remarks { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}