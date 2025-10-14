using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.PaymentModel.DTOs
{
    // ========================================
    // PAYMENT RECORDING (IMMUTABLE - NO EDIT/DELETE)
    // ========================================
    // Note: Payment methods (Cash, Bank, ABA, Wing, TrueMoney) are static/pre-configured in database
    
    public record class CreatePaymentDTO
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "USD"; // USD, KHR

        [Required]
        public Guid PaymentMethodId { get; set; } // Cash, Bank, ABA, Wing, TrueMoney

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public string PaymentFor { get; set; } // 'Registration', 'Tuition', 'Course Fee', 'Support Fee', 'Supply Fee', 'Enrollment Fee'

        [StringLength(50)]
        public string? Term { get; set; } // e.g. '2025-Term1', '2025-Term2' (if applicable)

        [StringLength(100)]
        public string? PayerName { get; set; } // Who paid (parent name, student name, etc)

        [StringLength(100)]
        public string? TransactionReference { get; set; } // Bank reference, ABA transaction ID, etc

        public string? Notes { get; set; }

        // Set by controller from claims
        public string? CreatedBy { get; set; }
    }

    public record class GetPaymentDTO
    {
        public Guid PaymentId { get; set; }
        public string PaymentReference { get; set; } // Auto-generated: PAY-2025-00001
        
        public Guid StudentId { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal? AmountInKHR { get; set; } // If USD, show KHR equivalent

        public Guid PaymentMethodId { get; set; }
        public string? PaymentMethodName { get; set; }

        public DateTime PaymentDate { get; set; }
        public string PaymentFor { get; set; }
        public string? Term { get; set; }

        public string? PayerName { get; set; }
        public string? TransactionReference { get; set; }
        public string? Notes { get; set; }

        public string Status { get; set; } // 'Completed', 'Voided'
        public string? VoidReason { get; set; }
        public DateTime? VoidedDate { get; set; }
        public string? VoidedBy { get; set; }

        public string? ReceivedBy { get; set; }
        public DateTime CreateDate { get; set; }
    }

    // ========================================
    // VOID PAYMENT (For Mistakes - Original Stays in System)
    // ========================================
    
    public record class VoidPaymentDTO
    {
        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        [StringLength(200)]
        public string VoidReason { get; set; } // 'Wrong Amount', 'Wrong Student', 'Duplicate Entry', 'Wrong Method', 'Other'

        public string? DetailedExplanation { get; set; }

        // Set by controller
        public string? VoidedBy { get; set; }
    }

    // ========================================
    // STUDENT PAYMENT HISTORY
    // ========================================
    
    public record class StudentPaymentHistoryDTO
    {
        public Guid StudentId { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        
        public decimal TotalPaidUSD { get; set; }
        public decimal TotalPaidKHR { get; set; }
        
        public List<GetPaymentDTO> Payments { get; set; } = new();
    }

    // ========================================
    // PAYMENT REPORTS
    // ========================================
    
    public record class DailyPaymentReportDTO
    {
        public DateTime ReportDate { get; set; }
        public int TotalTransactions { get; set; }
        public int CompletedTransactions { get; set; }
        public int VoidedTransactions { get; set; }
        
        public decimal TotalAmountUSD { get; set; }
        public decimal TotalAmountKHR { get; set; }
        
        public List<PaymentMethodSummaryDTO> ByPaymentMethod { get; set; } = new();
        public List<PaymentTypeSummaryDTO> ByPaymentType { get; set; } = new();
    }

    public record class PaymentMethodSummaryDTO
    {
        public string PaymentMethodName { get; set; }
        public int Count { get; set; }
        public decimal TotalUSD { get; set; }
        public decimal TotalKHR { get; set; }
    }

    public record class PaymentTypeSummaryDTO
    {
        public string PaymentFor { get; set; } // Registration, Tuition, etc
        public int Count { get; set; }
        public decimal TotalUSD { get; set; }
    }

    // ========================================
    // LIST DTOs (For UI Tables)
    // ========================================
    
    public record class ListPaymentDTOs
    {
        public PageList<GetPaymentDTO>? Payments { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchText { get; set; }
        public string? StatusFilter { get; set; }
        public Guid? StudentFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
