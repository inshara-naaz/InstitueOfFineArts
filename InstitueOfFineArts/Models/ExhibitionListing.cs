using InstituteOfFineArts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstituteOfFineArts.Models
{
    public class ExhibitionListing
    {
        [Key]
        public int ExhibitionListingId { get; set; }

        [Required]
        public int ExhibitionId { get; set; }

        [ForeignKey(nameof(ExhibitionId))]
        public Exhibition Exhibition { get; set; } = null!;

        [Required]
        public int SubmissionId { get; set; }

        [ForeignKey(nameof(SubmissionId))]
        public Submission Submission { get; set; } = null!;

        [Required]
        public int ForwardedByStaffId { get; set; }

        [ForeignKey(nameof(ForwardedByStaffId))]
        public Staff ForwardedByStaff { get; set; } = null!;

        // Price set by the student (painter)
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StudentQuotedPrice { get; set; }

        public SaleStatus SaleStatus { get; set; } = SaleStatus.Available;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? FinalSalePrice { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateSold { get; set; }

        // Buyer details (filled when painting is sold)
        [StringLength(150)]
        public string? BuyerName { get; set; }

        [Phone]
        [StringLength(20)]
        public string? BuyerPhone { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? BuyerEmail { get; set; }

        [StringLength(300)]
        public string? BuyerAddress { get; set; }

        // Payment tracking — money handed to student after sale
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotApplicable;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AmountPaidToStudent { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; }

        [StringLength(500)]
        public string? PaymentRemarks { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime ListedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum SaleStatus
    {
        Available = 1,
        Sold = 2,
        Returned = 3,
        Withdrawn = 4
    }

    public enum PaymentStatus
    {
        NotApplicable = 0,
        Pending = 1,
        Paid = 2
    }
}