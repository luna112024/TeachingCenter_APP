using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.StudentModel.DTOs
{
    public abstract record class StudentBaseDTO
    {
        [Required]
        [StringLength(20)]
        public string StudentCode { get; set; } = string.Empty;

        public Guid? CurrentLevelId { get; set; }
        public string? LearningGoals { get; set; }

        [StringLength(100)]
        public string? EmergencyContactName { get; set; }
        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }
        [StringLength(50)]
        public string? EmergencyContactRelationship { get; set; }

        [StringLength(100)]
        public string? GuardianName { get; set; }
        [StringLength(20)]
        public string? GuardianPhone { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? GuardianEmail { get; set; }
        [StringLength(20)]
        public string? GuardianRelationship { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public bool PreviousChineseStudy { get; set; } = false;
        [Range(0, int.MaxValue)]
        public int? StudyDurationYears { get; set; }
        [StringLength(20)]
        public string? PreferredClassTime { get; set; }
        [StringLength(20)]
        public string? StudentStatus { get; set; }
        public string? Notes { get; set; }
    }
    
    public record class CreateStudentDTO : StudentBaseDTO
    {
        public Guid? UserId { get; set; }
        public string? CreatedBy { get; set; }
    }

    public record class UpdateStudentDTO : StudentBaseDTO
    {
        [Required]
        public Guid StudentId { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetStudentDTO : StudentBaseDTO
    {
        public Guid StudentId { get; set; }
        public Guid? UserId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        // Additional fields for UI display
        public string? CurrentLevelName { get; set; }
        public string? CurrentLevelCode { get; set; }
        public string? CurrentLevelNameChinese { get; set; }
    }
}
