using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace InstituteOfFineArts.Models.ViewModels
{
    public class SubmissionCreateViewModel
    {
        [Required]
        [Display(Name = "Competition")]
        public int CompetitionId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Painting Title")]
        public string PaintingTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please upload your painting (JPEG only).")]
        [Display(Name = "Upload Painting (JPEG)")]
        public IFormFile PaintingFile { get; set; } = null!;

        [Required]
        [Display(Name = "Text Type")]
        public string TextType { get; set; } = "Description";

        [Required]
        [StringLength(5000)]
        [Display(Name = "Poem / Quotation / Story / Description")]
        public string AccompanyingText { get; set; } = string.Empty;

        [StringLength(3000)]
        [Display(Name = "Why did you create this painting?")]
        public string? CreativeDescription { get; set; }
    }

    public class SubmissionEditViewModel
    {
        public int SubmissionId { get; set; }
        public int CompetitionId { get; set; }

        [Display(Name = "Current Painting")]
        public string? ExistingImageFilePath { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Painting Title")]
        public string PaintingTitle { get; set; } = string.Empty;

        [Display(Name = "Replace Painting (JPEG) — Leave blank to keep current")]
        public IFormFile? NewPaintingFile { get; set; }

        [Required]
        [Display(Name = "Text Type")]
        public string TextType { get; set; } = "Description";

        [Required]
        [StringLength(5000)]
        [Display(Name = "Poem / Quotation / Story / Description")]
        public string AccompanyingText { get; set; } = string.Empty;

        [StringLength(3000)]
        [Display(Name = "Creative Intent / Description")]
        public string? CreativeDescription { get; set; }
    }

    public class SubmissionEvaluateViewModel
    {
        public int SubmissionId { get; set; }
        public string PaintingTitle { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string CompetitionTitle { get; set; } = string.Empty;
        public string ImageFilePath { get; set; } = string.Empty;
        public string AccompanyingText { get; set; } = string.Empty;
        public string TextType { get; set; } = string.Empty;
        public string? CreativeDescription { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsLateSubmission { get; set; }

        [Required]
        [Display(Name = "Grade")]
        public SubmissionGrade Grade { get; set; }

        [StringLength(2000)]
        [Display(Name = "Positive Points")]
        public string? PositivePoints { get; set; }

        [StringLength(2000)]
        [Display(Name = "Negative Points")]
        public string? NegativePoints { get; set; }

        [StringLength(2000)]
        [Display(Name = "Areas of Improvement")]
        public string? AreasOfImprovement { get; set; }

        [StringLength(2000)]
        [Display(Name = "General Remarks")]
        public string? GeneralRemarks { get; set; }
    }
}