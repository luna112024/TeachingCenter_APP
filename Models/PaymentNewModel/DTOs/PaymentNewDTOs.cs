using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.PaymentNewModel.DTOs
{
    // Create Payment DTO
    public class CreatePaymentNewDTO
    {
        [Required(ErrorMessage = "Invoice is required")]
        public Guid InvoiceId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment date is required")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = "Cash";

        public string? TransactionReference { get; set; }

        public string? BankReference { get; set; }

        public string? CheckNumber { get; set; }

        public string? PayerName { get; set; }

        public string? PayerRelationship { get; set; }

        public Guid? ReceivedBy { get; set; }

        public string? Notes { get; set; }

        public string? CreatedBy { get; set; }
    }

    // Confirm Payment DTO
    public class ConfirmPaymentDTO
    {
        [Required]
        public Guid ConfirmedBy { get; set; }

        public string? Notes { get; set; }
    }

    // Add Note DTO
    public class AddPaymentNoteDTO
    {
        [Required(ErrorMessage = "Note is required")]
        public string Note { get; set; } = string.Empty;

        public string? ModifiedBy { get; set; }
    }

    // Get Payment DTO
    public class GetPaymentNewDTO
    {
        public Guid PaymentId { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public Guid? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public Guid StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public TimeSpan? PaymentTime { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionReference { get; set; }
        public string? PayerName { get; set; }
        public string? PayerRelationship { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public DateTime? LockDate { get; set; }
        public string? ReceivedBy { get; set; }
        public string? ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public string? Notes { get; set; }
        public string? InternalComments { get; set; }
        public List<PaymentAllocationDTO> Allocations { get; set; } = new List<PaymentAllocationDTO>();
        public decimal? OriginalAmount { get; set; }
        public string? AdjustmentReason { get; set; }
        public Guid? RelatedPaymentId { get; set; }
        public string? RelatedPaymentReference { get; set; }
        public DateTime CreateDate { get; set; }
    }

    // Payment Allocation DTO
    public class PaymentAllocationDTO
    {
        public Guid AllocationId { get; set; }
        public Guid InvoiceDetailId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal AllocatedAmount { get; set; }
        public DateTime AllocationDate { get; set; }
    }

    // Payment Audit DTO
    public class PaymentAuditDTO
    {
        public Guid AuditId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedDate { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public decimal? OldAmount { get; set; }
        public decimal? NewAmount { get; set; }
        public string? Reason { get; set; }
    }

    // Create Payment Adjustment DTO
    public class CreatePaymentAdjustmentDTO
    {
        [Required]
        public Guid OriginalPaymentId { get; set; }

        [Required]
        public decimal AdjustmentAmount { get; set; }

        [Required]
        public string AdjustmentReason { get; set; } = string.Empty;

        [Required]
        public Guid AdjustedBy { get; set; }

        public string? CreatedBy { get; set; }
    }
}

