using System.ComponentModel.DataAnnotations;

namespace InstituteOfFineArts.Models.ViewModels
{
    public class ExhibitionListingCreateViewModel
    {
        [Required]
        [Display(Name = "Exhibition")]
        public int ExhibitionId { get; set; }

        [Required]
        [Display(Name = "Submission / Painting")]
        public int SubmissionId { get; set; }

        [Required]
        [Range(0.01, 9999999)]
        [DataType(DataType.Currency)]
        [Display(Name = "Student's Quoted Price")]
        public decimal StudentQuotedPrice { get; set; }

        [StringLength(1000)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }

    public class ExhibitionListingUpdateViewModel
    {
        public int ExhibitionListingId { get; set; }

        [Display(Name = "Sale Status")]
        public SaleStatus SaleStatus { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Final Sale Price")]
        public decimal? FinalSalePrice { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Sold")]
        public DateTime? DateSold { get; set; }

        [StringLength(150)]
        [Display(Name = "Buyer Name")]
        public string? BuyerName { get; set; }

        [Phone]
        [StringLength(20)]
        [Display(Name = "Buyer Phone")]
        public string? BuyerPhone { get; set; }

        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Buyer Email")]
        public string? BuyerEmail { get; set; }

        [StringLength(300)]
        [Display(Name = "Buyer Address")]
        public string? BuyerAddress { get; set; }

        [Display(Name = "Payment Status to Student")]
        public PaymentStatus PaymentStatus { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Amount Paid to Student")]
        public decimal? AmountPaidToStudent { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Payment Remarks")]
        public string? PaymentRemarks { get; set; }
    }

    public class StudentExhibitionStatusViewModel
    {
        public string ExhibitionName { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime ExhibitionStartDate { get; set; }
        public DateTime ExhibitionEndDate { get; set; }
        public string PaintingTitle { get; set; } = string.Empty;
        public string ImageFilePath { get; set; } = string.Empty;
        public decimal StudentQuotedPrice { get; set; }
        public SaleStatus SaleStatus { get; set; }
        public decimal? FinalSalePrice { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal? AmountPaidToStudent { get; set; }
    }
}