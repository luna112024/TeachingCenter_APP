using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.TeacherModel.DTOs
{
    public abstract record class TeacherBaseDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string TeacherCode { get; set; } = string.Empty;

        public string? Specializations { get; set; }

        public string? Qualifications { get; set; }

        [Range(0, int.MaxValue)]
        public int? ExperienceYears { get; set; }

        [Required]
        [StringLength(20)]
        public string ChineseProficiency { get; set; } = string.Empty;

        public string? TeachingLanguages { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxHoursPerWeek { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? HourlyRate { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [StringLength(20)]
        public string? ContractType { get; set; }

        [StringLength(20)]
        public string? EmploymentStatus { get; set; }

        public string? Notes { get; set; }
    }

    public record class CreateTeacherDTO : TeacherBaseDTO
    {
        public string? CreatedBy { get; set; }
    }

    public record class UpdateTeacherDTO : TeacherBaseDTO
    {
        [Required]
        public Guid TeacherId { get; set; }
        public string? ModifiedBy { get; set; }

    }

    public record class GetTeacherDTO : TeacherBaseDTO
    {
        public Guid TeacherId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        // Additional fields for UI display
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
    }

    public record class TeacherScheduleItemDTO
    {
        public string? SectionCode { get; set; }
        public string? SchedulePattern { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
    }

    public record class TeacherWorkloadDTO
    {
        public int CurrentLoad { get; set; }
        public int MaxHoursPerWeek { get; set; }
        public double UtilizationPercentage { get; set; }
    }
}