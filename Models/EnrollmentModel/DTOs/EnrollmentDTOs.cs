using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.EnrollmentModel.DTOs
{
    public abstract record class EnrollmentBaseDTO
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public Guid SectionId { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; }

        [StringLength(20)]
        public string EnrollmentType { get; set; } = "Regular"; // 'Regular', 'Trial', 'Makeup', 'Transfer'

        [StringLength(20)]
        public string Status { get; set; } = "Enrolled"; // 'Enrolled', 'Active', 'Completed', 'Dropped', 'Failed', 'Transferred'

        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? FinalGrade { get; set; }
        public decimal? FinalScore { get; set; }
        public decimal? AttendanceRate { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; } = 0.00m;

        [StringLength(200)]
        public string? DiscountReason { get; set; }

        public string? SpecialNeeds { get; set; }
        public string? Notes { get; set; }
    }

    public record class CreateEnrollmentDTO : EnrollmentBaseDTO
    {
        public string? CreatedBy { get; set; }
    }

    public record class UpdateEnrollmentDTO : EnrollmentBaseDTO
    {
        [Required]
        public Guid EnrollmentId { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetEnrollmentDTO : EnrollmentBaseDTO
    {
        public Guid EnrollmentId { get; set; }
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

    public record class TransferEnrollmentDTO
    {
        [Required]
        public Guid NewSectionId { get; set; }

        [Required]
        public DateTime TransferDate { get; set; }

        [StringLength(200)]
        public string? TransferReason { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
