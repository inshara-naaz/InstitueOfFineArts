using System.ComponentModel.DataAnnotations;

namespace InstituteOfFineArts.Models.ViewModels
{
    public class CompetitionCreateViewModel
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Competition Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [StringLength(3000)]
        [Display(Name = "Conditions / Rules")]
        public string? Conditions { get; set; }

        // ── Award Tiers for this Competition ───────────────────────────────────
        [Display(Name = "Award Tiers")]
        public List<CompetitionAwardInputModel> Awards { get; set; } = new()
        {
            new CompetitionAwardInputModel { Rank = 1, AwardTitle = "1st Prize" },
            new CompetitionAwardInputModel { Rank = 2, AwardTitle = "2nd Prize" },
            new CompetitionAwardInputModel { Rank = 3, AwardTitle = "3rd Prize" }
        };
    }

    public class CompetitionEditViewModel
    {
        public int CompetitionId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Competition Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [StringLength(3000)]
        [Display(Name = "Conditions / Rules")]
        public string? Conditions { get; set; }

        [Display(Name = "Status")]
        public CompetitionStatus Status { get; set; }

        [Display(Name = "Award Tiers")]
        public List<CompetitionAwardInputModel> Awards { get; set; } = new();
    }

    /// <summary>
    /// Represents a single award tier input row inside the competition form.
    /// e.g. Rank 1 = "1st Prize", Rs. 5000
    /// </summary>
    public class CompetitionAwardInputModel
    {
        public int? CompetitionAwardId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Award Title")]
        public string AwardTitle { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Award Description")]
        public string? AwardDescription { get; set; }

        [Range(0, 9999999)]
        [DataType(DataType.Currency)]
        [Display(Name = "Prize Money")]
        public decimal? PrizeMoney { get; set; }

        [Required]
        [Display(Name = "Rank")]
        public int Rank { get; set; }
    }

    public class CompetitionDetailsViewModel
    {
        public int CompetitionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Conditions { get; set; }
        public CompetitionStatus Status { get; set; }
        public string ManagedByStaff { get; set; } = string.Empty;
        public List<CompetitionAward> Awards { get; set; } = new();
        public int TotalSubmissions { get; set; }
    }
}