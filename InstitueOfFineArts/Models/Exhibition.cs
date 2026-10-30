using InstituteOfFineArts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstituteOfFineArts.Models
{
    public class Exhibition
    {
        [Key]
        public int ExhibitionId { get; set; }

        [Required]
        [StringLength(300)]
        public string ExhibitionName { get; set; } = string.Empty;

        [StringLength(3000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(300)]
        public string Venue { get; set; } = string.Empty;

        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [StringLength(100)]
        public string Country { get; set; } = "Pakistan";

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExhibitionStartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExhibitionEndDate { get; set; }
        

        public ExhibitionStatus Status { get; set; } = ExhibitionStatus.Upcoming;

        [Required]
        public int StaffId { get; set; }

        [ForeignKey(nameof(StaffId))]
        public Staff ManagedByStaff { get; set; } = null!;

        [StringLength(1000)]
        public string? AdditionalNotes { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ExhibitionListing> ExhibitionListings { get; set; } = new List<ExhibitionListing>();
    }

    public enum ExhibitionStatus
    {
        Upcoming = 1,
        Ongoing = 2,
        Concluded = 3,
        Cancelled = 4
    }
}