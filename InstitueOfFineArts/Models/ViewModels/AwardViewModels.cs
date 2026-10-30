using System.ComponentModel.DataAnnotations;

namespace InstituteOfFineArts.Models.ViewModels
{
    public class AwardCreateViewModel
    {
        [Required]
        [Display(Name = "Competition")]
        public int CompetitionId { get; set; }

        [Required]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Winning Submission")]
        public int SubmissionId { get; set; }

        [Display(Name = "Award Tier")]
        public int? CompetitionAwardId { get; set; }

        [StringLength(200)]
        [Display(Name = "Custom Award Title (if not using a tier)")]
        public string? CustomAwardTitle { get; set; }

        [StringLength(2000)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date Issued")]
        public DateTime DateIssued { get; set; } = DateTime.Today;
    }

    public class AwardEditViewModel
    {
        public int AwardId { get; set; }

        [Display(Name = "Award Tier")]
        public int? CompetitionAwardId { get; set; }

        [StringLength(200)]
        [Display(Name = "Custom Award Title")]
        public string? CustomAwardTitle { get; set; }

        [StringLength(2000)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date Issued")]
        public DateTime DateIssued { get; set; }
    }

    public class AwardDetailsViewModel
    {
        public int AwardId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CompetitionTitle { get; set; } = string.Empty;
        public string PaintingTitle { get; set; } = string.Empty;
        public string? AwardTier { get; set; }
        public string? CustomAwardTitle { get; set; }
        public decimal? PrizeMoney { get; set; }
        public string? Remarks { get; set; }
        public DateTime DateIssued { get; set; }
        public string IssuedByStaff { get; set; } = string.Empty;
    }
}
