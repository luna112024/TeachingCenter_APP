using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.StudentModel.DTOs
{
    public abstract record class StudentBaseDTO
    {
        [Required]
        [StringLength(20)]
        public string StudentCode { get; set; } = string.Empty;

        // Personal Information
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? KhmerName { get; set; }

        [StringLength(100)]
        public string? ChineseName { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; } // 'Male', 'Female', 'Other'

        public DateTime? DateOfBirth { get; set; }

        [StringLength(50)]
        public string? Nationality { get; set; }

        [StringLength(50)]
        public string? IdCardNumber { get; set; } // ID Card or Passport Number

        // Contact Information
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Province { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        // Academic Information
        [StringLength(20)]
        public string? CurrentLevel { get; set; } // Same values as Course Level

        public string? LearningGoals { get; set; }

        public bool PreviousChineseStudy { get; set; } = false;

        [Range(0, int.MaxValue)]
        public int StudyDurationYears { get; set; } = 0;

        [StringLength(20)]
        public string PreferredClassTime { get; set; } = "Flexible"; // 'Morning', 'Afternoon', 'Evening', 'Weekend', 'Flexible'

        // Emergency Contact
        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(50)]
        public string? EmergencyContactRelationship { get; set; }

        // Guardian Information (for minors)
        [StringLength(100)]
        public string? GuardianName { get; set; }

        [StringLength(20)]
        public string? GuardianPhone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? GuardianEmail { get; set; }

        [StringLength(20)]
        public string? GuardianRelationship { get; set; } // 'Parent', 'Guardian', 'Relative', 'Other'

        [StringLength(50)]
        public string? GuardianIdCard { get; set; }

        [StringLength(100)]
        public string? GuardianOccupation { get; set; }

        public string? GuardianAddress { get; set; }

        // Enrollment Information
        [Required]
        public DateTime EnrollmentDate { get; set; }

        public DateTime? GraduationDate { get; set; }

        // Profile Photo (Optional)
        [StringLength(500)]
        public string? ProfilePhoto { get; set; } // File path or URL

        // Status
        [StringLength(20)]
        public string StudentStatus { get; set; } = "Active"; // 'Active', 'Graduated', 'Dropped', 'Suspended', 'On Hold'

        public string? Notes { get; set; }
    }
    
    public record class CreateStudentDTO : StudentBaseDTO
    {
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
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        
        // Additional fields for UI display
        public string? CurrentLevelName { get; set; }
        public string? CurrentLevelCode { get; set; }
        public string? CurrentLevelNameChinese { get; set; }
    }
}
