using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.AttendanceModel.DTOs
{
    public abstract record class AttendanceBaseDTO
    {
        [Required]
        public DateTime ClassDate { get; set; }

        [Required]
        public TimeSpan ScheduledStartTime { get; set; }

        [Required]
        public TimeSpan ScheduledEndTime { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty; // 'Present', 'Absent', 'Late', 'Excused', 'Sick', 'Emergency'

        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public int MinutesLate { get; set; } = 0;

        public bool MakeupRequired { get; set; } = false;
        public DateTime? MakeupScheduledDate { get; set; }
        public bool HomeworkSubmitted { get; set; } = true;
        public int ParticipationScore { get; set; } = 5; // 1-5 scale

        public string? Notes { get; set; }
    }

    public record class CreateAttendanceDTO : AttendanceBaseDTO
    {
        [Required]
        public Guid EnrollmentId { get; set; }

        public Guid? RecordedBy { get; set; }
    }

    public record class UpdateAttendanceDTO : AttendanceBaseDTO
    {
        [Required]
        public Guid AttendanceId { get; set; }

        public string? ModifiedBy { get; set; }
    }

    public record class GetAttendanceDTO : AttendanceBaseDTO
    {
        public Guid AttendanceId { get; set; }
        public Guid EnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public Guid ClassSectionId { get; set; }
        public Guid? RecordedBy { get; set; }
        public DateTime? RecordedAt { get; set; }

        // Additional fields for UI display
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public string? SectionCode { get; set; }
        public string? SectionName { get; set; }
        public string? CourseName { get; set; }
        public string? TeacherName { get; set; }
    }

    // Bulk Attendance Operations
    public record class BulkAttendanceDTO
    {
        [Required]
        public Guid SectionId { get; set; }

        [Required]
        public DateTime ClassDate { get; set; }

        [Required]
        public TimeSpan ScheduledStartTime { get; set; }

        [Required]
        public TimeSpan ScheduledEndTime { get; set; }

        [Required]
        public List<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

        public string? CreatedBy { get; set; }
    }

    public record class AttendanceRecord
    {
        [Required]
        public Guid EnrollmentId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public int MinutesLate { get; set; } = 0;
        public bool MakeupRequired { get; set; } = false;
        public DateTime? MakeupScheduledDate { get; set; }
        public bool HomeworkSubmitted { get; set; } = true;
        public int ParticipationScore { get; set; } = 5;
        public string? Notes { get; set; }
    }

    // Attendance Reports
    public record class AttendanceReportDTO
    {
        public Guid SectionId { get; set; }
        public string? SectionCode { get; set; }
        public string? SectionName { get; set; }
        public string? CourseName { get; set; }
        public string? TermName { get; set; }
        public string? TeacherName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<StudentAttendanceDTO> StudentAttendances { get; set; } = new List<StudentAttendanceDTO>();
        public AttendanceStatisticsDTO Statistics { get; set; } = new AttendanceStatisticsDTO();
    }

    public record class StudentAttendanceDTO
    {
        public Guid StudentId { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public int TotalClasses { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int ExcusedCount { get; set; }
        public decimal AttendanceRate { get; set; }
        public List<AttendanceDetailDTO> AttendanceDetails { get; set; } = new List<AttendanceDetailDTO>();
    }

    public record class AttendanceDetailDTO
    {
        public DateTime ClassDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public int MinutesLate { get; set; }
        public string? Notes { get; set; }
    }

    public record class AttendanceStatisticsDTO
    {
        public int TotalStudents { get; set; }
        public decimal AverageAttendanceRate { get; set; }
        public int StudentsAboveThreshold { get; set; }
        public int StudentsBelowThreshold { get; set; }
        public decimal AttendanceThreshold { get; set; } = 80.0m;
        public List<AttendanceAlertDTO> Alerts { get; set; } = new List<AttendanceAlertDTO>();
    }

    // Attendance Alerts
    public record class AttendanceAlertDTO
    {
        public Guid StudentId { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public Guid SectionId { get; set; }
        public string? SectionCode { get; set; }
        public string? AlertType { get; set; } = string.Empty; // 'Low Attendance', 'Consecutive Absences', 'Frequent Late'
        public string? AlertMessage { get; set; }
        public int ConsecutiveAbsences { get; set; }
        public decimal CurrentAttendanceRate { get; set; }
        public DateTime LastAttendanceDate { get; set; }
        public string? RecommendedAction { get; set; }
    }

    // Attendance Summary for Dashboard
    public record class AttendanceSummaryDTO
    {
        public DateTime Date { get; set; }
        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public decimal OverallAttendanceRate { get; set; }
        public List<SectionAttendanceDTO> SectionAttendances { get; set; } = new List<SectionAttendanceDTO>();
    }

    public record class SectionAttendanceDTO
    {
        public Guid SectionId { get; set; }
        public string? SectionCode { get; set; }
        public string? SectionName { get; set; }
        public string? CourseName { get; set; }
        public string? TeacherName { get; set; }
        public int StudentCount { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public decimal AttendanceRate { get; set; }
    }

    // Additional DTOs for repository compatibility
    public record class StudentAttendanceSummaryDTO
    {
        public Guid StudentId { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLate { get; set; }
        public decimal AttendanceRate { get; set; }
    }

    public record class SectionFilterDTO
    {
        public Guid SectionId { get; set; }
        public string? SectionName { get; set; }
        public string? CourseName { get; set; }
    }

    // Attendance Filter DTOs
    public record class AttendanceFilterDTO
    {
        public List<SectionFilterDTO> Sections { get; set; } = new List<SectionFilterDTO>();
        public string[] StatusOptions { get; set; } = Array.Empty<string>();
    }

    // Request DTOs for specific operations
    public record class ScheduleMakeupRequest
    {
        public DateTime MakeupDate { get; set; }
        public string? Notes { get; set; }
    }

    public record class CheckAbsencesRequest
    {
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
    }
}
