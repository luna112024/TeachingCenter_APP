using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.WaitlistModel.DTOs
{
    public abstract record class WaitlistBaseDTO
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public Guid SectionId { get; set; }

        [Required]
        public DateTime WaitlistDate { get; set; }

        [Range(1, int.MaxValue)]
        public int PriorityOrder { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Waiting"; // 'Waiting', 'Enrolled', 'Expired', 'Cancelled'

        public string? Notes { get; set; }
    }

    public record class CreateWaitlistDTO : WaitlistBaseDTO
    {
        public string? CreatedBy { get; set; }
    }

    public record class UpdateWaitlistDTO : WaitlistBaseDTO
    {
        [Required]
        public Guid WaitlistId { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetWaitlistDTO : WaitlistBaseDTO
    {
        public Guid WaitlistId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Navigation properties
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public string? SectionName { get; set; }
        public string? SectionCode { get; set; }
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public string? TermName { get; set; }
        public string? TeacherName { get; set; }
        public string? ClassroomName { get; set; }
    }

    public record class ReorderWaitlistDTO
    {
        [Required]
        public Guid SectionId { get; set; }

        [Required]
        public List<WaitlistOrderItem> WaitlistOrder { get; set; } = new();
    }

    public record class WaitlistOrderItem
    {
        [Required]
        public Guid WaitlistId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PriorityOrder { get; set; }
    }

    public record class PromoteFromWaitlistDTO
    {
        [Required]
        public DateTime EnrollmentDate { get; set; }

        [StringLength(20)]
        public string EnrollmentType { get; set; } = "Regular";

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; } = 0.00m;

        [StringLength(200)]
        public string? DiscountReason { get; set; }

        public string? SpecialNeeds { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
    }
}
