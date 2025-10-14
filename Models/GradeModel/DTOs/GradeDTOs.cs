using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.GradeModel.DTOs
{
    public abstract record class GradeBaseDTO
    {
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal EarnedScore { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Percentage { get; set; }

        [StringLength(5)]
        public string? LetterGrade { get; set; }

        public string? Strengths { get; set; }
        public string? AreasForImprovement { get; set; }
        public string? TeacherComments { get; set; }

        [Required]
        public DateTime GradedDate { get; set; }

        public bool LateSubmission { get; set; } = false;
        public bool ResubmissionAllowed { get; set; } = false;
    }

    public record class CreateGradeDTO : GradeBaseDTO
    {
        [Required]
        public Guid EnrollmentId { get; set; }

        [Required]
        public Guid AssessmentId { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        public string? GradedBy { get; set; }
    }

    public record class UpdateGradeDTO : GradeBaseDTO
    {
        [Required]
        public Guid GradeId { get; set; }

        public string? GradedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetGradeDTO : GradeBaseDTO
    {
        public Guid GradeId { get; set; }
        public Guid EnrollmentId { get; set; }
        public Guid AssessmentId { get; set; }
        public string? GradedBy { get; set; }
        public string? GradedByUsername { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Additional fields for UI display
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public string? AssessmentName { get; set; }
        public string? AssessmentType { get; set; }
        public decimal MaxScore { get; set; }
        public string? SectionName { get; set; }
        public string? CourseName { get; set; }
    }

    public record class BulkGradeEntryDTO
    {
        [Required]
        public Guid AssessmentId { get; set; }

        [Required]
        public List<GradeEntryItem> Grades { get; set; } = new List<GradeEntryItem>();

        public string? GradedBy { get; set; }
    }

    public record class GradeEntryItem
    {
        [Required]
        public Guid EnrollmentId { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal EarnedScore { get; set; }

        public string? Strengths { get; set; }
        public string? AreasForImprovement { get; set; }
        public string? TeacherComments { get; set; }
        public string? GradedBy { get; set; }
        public bool LateSubmission { get; set; } = false;
        public bool ResubmissionAllowed { get; set; } = false;
    }

    public record class FinalGradeCalculationDTO
    {
        [Required]
        public Guid SectionId { get; set; }

        public string? CalculatedBy { get; set; }
    }

    public record class GradeReportDTO
    {
        public Guid SectionId { get; set; }
        public string? SectionCode { get; set; }
        public string? SectionName { get; set; }
        public string? CourseName { get; set; }
        public string? TermName { get; set; }
        public int TotalStudents { get; set; }
        public List<AssessmentGradeSummaryDTO> Assessments { get; set; } = new List<AssessmentGradeSummaryDTO>();
        public List<StudentGradeDTO> StudentGrades { get; set; } = new List<StudentGradeDTO>();
        public GradeStatisticsDTO Statistics { get; set; } = new GradeStatisticsDTO();
    }

    public record class StudentGradeDTO
    {
        public Guid GradeId { get; set; }
        public Guid StudentId { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public string? AssessmentName { get; set; }
        public string? AssessmentType { get; set; }
        public string? SectionName { get; set; }
        public string? CourseName { get; set; }
        public decimal EarnedScore { get; set; }
        public decimal MaxScore { get; set; }
        public decimal Percentage { get; set; }
        public string? LetterGrade { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public DateTime? GradedDate { get; set; }
        public decimal FinalScore { get; set; }
        public string? FinalLetterGrade { get; set; }
        public List<AssessmentGradeDTO> AssessmentGrades { get; set; } = new List<AssessmentGradeDTO>();
    }

    public record class AssessmentGradeDTO
    {
        public Guid AssessmentId { get; set; }
        public string? AssessmentName { get; set; }
        public string? AssessmentType { get; set; }
        public decimal MaxScore { get; set; }
        public decimal EarnedScore { get; set; }
        public decimal Percentage { get; set; }
        public string? LetterGrade { get; set; }
        public decimal WeightPercentage { get; set; }
        public decimal WeightedScore { get; set; }
    }

    public record class GradeStatisticsDTO
    {
        public Guid SectionId { get; set; }
        public int TotalStudents { get; set; }
        public int TotalGrades { get; set; }
        public decimal AverageScore { get; set; }
        public decimal HighestScore { get; set; }
        public decimal LowestScore { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public decimal PassRate { get; set; }
        public Dictionary<string, int> GradeDistribution { get; set; } = new Dictionary<string, int>();
    }

    public record class AssessmentGradeSummaryDTO
    {
        public Guid AssessmentId { get; set; }
        public string? AssessmentName { get; set; }
        public string? AssessmentType { get; set; }
        public decimal MaxScore { get; set; }
        public decimal Weight { get; set; }
        public double AverageScore { get; set; }
        public int CompletedCount { get; set; }
    }

    public record class StudentGradeSummaryDTO
    {
        public Guid StudentId { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public string? FinalGrade { get; set; }
        public decimal FinalScore { get; set; }
    }
}
