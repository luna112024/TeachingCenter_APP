using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.StudentCourseModel.DTOs
{
    // Assign Student to Course DTO
    public class AssignStudentToCourseDTO
    {
        [Required(ErrorMessage = "Student is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Course is required")]
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Term is required")]
        public Guid TermId { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public string AssignmentType { get; set; } = "NewStudent";

        public string? Notes { get; set; }

        public string? CreatedBy { get; set; }
    }

    // Promote Student DTO
    public class PromoteStudentDTO
    {
        [Required(ErrorMessage = "Student is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "New Course is required")]
        public Guid NewCourseId { get; set; }

        [Required(ErrorMessage = "New Term is required")]
        public Guid NewTermId { get; set; }

        public string PromotionStatus { get; set; } = "Promoted";

        public string? PerformanceNotes { get; set; }

        public string? CreatedBy { get; set; }
    }

    // Preview Promotion DTO
    public class PreviewPromotionDTO
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public Guid NewCourseId { get; set; }

        [Required]
        public Guid NewTermId { get; set; }
    }

    // Get Student Course DTO
    public class GetStudentCourseDTO
    {
        public Guid StudentCourseId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public decimal CourseFee { get; set; }
        public Guid TermId { get; set; }
        public string TermName { get; set; } = string.Empty;
        public DateTime AssignmentDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AssignmentType { get; set; } = string.Empty;
        public Guid? PreviousCourseId { get; set; }
        public string? PreviousCourseName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PromotionStatus { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public DateTime CreateDate { get; set; }

        // Invoice Information
        public Guid? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal? InvoiceTotal { get; set; }
        public string? InvoiceStatus { get; set; }
    }

    // Promotion Preview Response
    public class PromotionPreviewDTO
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CurrentCourse { get; set; } = string.Empty;
        public string TargetCourse { get; set; } = string.Empty;
        public decimal OutstandingBalance { get; set; }
        public decimal LateFeeAmount { get; set; }
        public decimal NewCourseFee { get; set; }
        public decimal EstimatedInvoiceTotal { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }

    // Bulk Promotion DTO
    public class BulkPromoteDTO
    {
        [Required]
        public Guid NewCourseId { get; set; }

        [Required]
        public Guid NewTermId { get; set; }

        [Required]
        public List<Guid> StudentIds { get; set; } = new List<Guid>();

        public string PromotionStatus { get; set; } = "Promoted";

        public string? Notes { get; set; }

        public string? CreatedBy { get; set; }
    }

    // Bulk Promotion Result
    public class BulkPromotionResultDTO
    {
        public int TotalStudents { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
        public List<StudentPromotionResult> Results { get; set; } = new List<StudentPromotionResult>();
    }

    public class StudentPromotionResult
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? Error { get; set; }
    }
}

