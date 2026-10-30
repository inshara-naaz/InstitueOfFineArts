using InstituteOfFineArts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstituteOfFineArts.Models
{
    public class Evaluation
    {
        [Key]
        public int EvaluationId { get; set; }

        // 1-to-1 with Submission
        [Required]
        public int SubmissionId { get; set; }

        [ForeignKey(nameof(SubmissionId))]
        public Submission Submission { get; set; } = null!;

        [Required]
        public int StaffId { get; set; }

        [ForeignKey(nameof(StaffId))]
        public Staff EvaluatedByStaff { get; set; } = null!;

        [Required]
        public SubmissionGrade Grade { get; set; }

        [StringLength(2000)]
        public string? PositivePoints { get; set; }

        [StringLength(2000)]
        public string? NegativePoints { get; set; }

        [StringLength(2000)]
        public string? AreasOfImprovement { get; set; }

        [StringLength(2000)]
        public string? GeneralRemarks { get; set; }

        public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}