using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.FeeModel.DTOs
{
    // Fee Categories
    public abstract record class FeeCategoryBaseDTO
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [Required]
        [StringLength(20)]
        public string CategoryCode { get; set; }

        public string? Description { get; set; }
        public bool IsMandatory { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
        public string Status { get; set; } = "Active";
    }

    public record class CreateFeeCategoryDTO : FeeCategoryBaseDTO
    {
        public string? CreatedBy { get; set; }
    }

    public record class UpdateFeeCategoryDTO : FeeCategoryBaseDTO
    {
        [Required]
        public Guid CategoryId { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetFeeCategoryDTO : FeeCategoryBaseDTO
    {
        public Guid CategoryId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    // Fee Templates
    public abstract record class FeeTemplateBaseDTO
    {
        [Required]
        [StringLength(200)]
        public string TemplateName { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(20)]
        public string FeeType { get; set; } // 'Tuition', 'Registration', 'Materials', 'Exam', 'Certificate', 'Late Fee', 'Deposit', 'Other'

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal BaseAmount { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        [StringLength(50)]
        public string ApplicableTo { get; set; } = "All"; // 'All', 'New Students', 'Returning Students', 'Specific Levels'

        public string? ApplicableLevels { get; set; } // JSON string for levels array

        public int DueDaysAfterEnrollment { get; set; } = 7;
        public int LateFeeDays { get; set; } = 7;
        public decimal LateFeeAmount { get; set; } = 0.00m;

        public decimal EarlyPaymentDiscountPercent { get; set; } = 0.00m;
        public int EarlyPaymentDays { get; set; } = 0;
        public decimal SiblingDiscountPercent { get; set; } = 0.00m;

        [Required]
        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = "Active";
    }

    public record class CreateFeeTemplateDTO : FeeTemplateBaseDTO
    {
        public string? CreatedBy { get; set; }
    }

    public record class UpdateFeeTemplateDTO : FeeTemplateBaseDTO
    {
        [Required]
        public Guid TemplateId { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetFeeTemplateDTO : FeeTemplateBaseDTO
    {
        public Guid TemplateId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CategoryName { get; set; }
    }

    // Student Fee Assignments
    public abstract record class StudentFeeBaseDTO
    {
        [Required]
        public Guid StudentId { get; set; }

        public Guid? EnrollmentId { get; set; }
        public Guid? TemplateId { get; set; }

        [Required]
        [StringLength(200)]
        public string FeeName { get; set; }

        [Required]
        [StringLength(20)]
        public string FeeType { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal OriginalAmount { get; set; }

        public decimal DiscountAmount { get; set; } = 0.00m;
        public string? DiscountReason { get; set; }

        [Required]
        public decimal FinalAmount { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        [Required]
        public DateTime DueDate { get; set; }

        public int GracePeriodDays { get; set; } = 0;
        public decimal LateFeeApplied { get; set; } = 0.00m;

        public string Status { get; set; } = "Pending"; // 'Pending', 'Partial', 'Paid', 'Overdue', 'Waived', 'Cancelled'

        public decimal AmountPaid { get; set; } = 0.00m;
        public decimal AmountOutstanding { get; set; }

        [StringLength(50)]
        public string? InvoiceNumber { get; set; }

        public string? Notes { get; set; }
    }

    public record class CreateStudentFeeDTO : StudentFeeBaseDTO
    {
        public string? CreatedBy { get; set; }
    }

    public record class UpdateStudentFeeDTO : StudentFeeBaseDTO
    {
        [Required]
        public Guid StudentFeeId { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetStudentFeeDTO : StudentFeeBaseDTO
    {
        public Guid StudentFeeId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? TemplateName { get; set; }
        public string? CategoryName { get; set; }
        public string? WaivedBy { get; set; }
        public DateTime? WaivedDate { get; set; }
        public string? WaivedReason { get; set; }

        // Additional fields for UI display
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public string? CourseName { get; set; }
        public string? SectionCode { get; set; }
    }

    public record class WaiveStudentFeeDTO
    {
        [Required]
        public Guid StudentFeeId { get; set; }

        [Required]
        public string WaiveReason { get; set; }

        [Required]
        public string WaivedReason { get; set; }

        public string? WaivedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class AssignFeeToStudentDTO
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public Guid TemplateId { get; set; }

        public Guid? EnrollmentId { get; set; }
        
        [Required, StringLength(200)]
        public string FeeName { get; set; } = string.Empty;
        
        [Required, StringLength(20)]
        public string FeeType { get; set; } = string.Empty;
        
        public decimal? CustomAmount { get; set; }
        public string? CustomDueDate { get; set; }
        public string? Notes { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; } = 0.00m;
        public string? DiscountReason { get; set; }
        public decimal FinalAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime DueDate { get; set; }
        public int GracePeriodDays { get; set; } = 0;
        public decimal LateFeeApplied { get; set; } = 0.00m;
        public string Status { get; set; } = "Pending";
        public decimal AmountPaid { get; set; } = 0.00m;
        public decimal AmountOutstanding { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
