using InstituteOfFineArts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstituteOfFineArts.Models
{
    public class CompetitionAward
    {
        [Key]
        public int CompetitionAwardId { get; set; }

        [Required]
        public int CompetitionId { get; set; }

        [ForeignKey(nameof(CompetitionId))]
        public Competition Competition { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string AwardTitle { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AwardDescription { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrizeMoney { get; set; }

        public int Rank { get; set; }

        public ICollection<Award> IssuedAwards { get; set; } = new List<Award>();
    }
}