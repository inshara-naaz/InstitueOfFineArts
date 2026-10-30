using InstituteOfFineArts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstituteOfFineArts.Models
{
    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        [Required]
        public int CompetitionId { get; set; }

        [ForeignKey(nameof(CompetitionId))]
        public Competition Competition { get; set; } = null!;

        // File I/O — stored path to the JPEG painting on the server
        [Required]
        [StringLength(500)]
        public string ImageFilePath { get; set; } = string.Empty;

        [StringLength(255)]
        public string? OriginalFileName { get; set; }

        public long FileSizeInBytes { get; set; }

        [Required]
        [StringLength(200)]
        public string PaintingTitle { get; set; } = string.Empty;

        // Poem / Quotation / Story / Description submitted with the painting
        [Required]
        [StringLength(5000)]
        public string AccompanyingText { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TextType { get; set; } = "Description"; // Poem | Quotation | Story | Description

        [StringLength(3000)]
        public string? CreativeDescription { get; set; }

        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastUpdatedAt { get; set; }

        // True if submitted after competition EndDate — auto-Disqualified
        public bool IsLateSubmission { get; set; } = false;

        public SubmissionGrade Grade { get; set; } = SubmissionGrade.Pending;

        public Evaluation? Evaluation { get; set; }

        public ICollection<ExhibitionListing> ExhibitionListings { get; set; } = new List<ExhibitionListing>();
    }

    public enum SubmissionGrade
    {
        Pending = 0,
        Best = 1,
        Better = 2,
        Good = 3,
        Moderate = 4,
        Normal = 5,
        Disqualified = 6
    }
}