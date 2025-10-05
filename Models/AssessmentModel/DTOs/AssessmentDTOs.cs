using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.AssessmentModel.DTOs
{
    public abstract record class AssessmentBaseDTO
    {
        [Required]
        [StringLength(200)]
        public string AssessmentName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AssessmentType { get; set; } = string.Empty; // 'Quiz', 'Test', 'Assignment', 'Project', 'Presentation', 'Midterm', 'Final', 'HSK Mock'

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal MaxScore { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal WeightPercentage { get; set; }

        [Required]
        public DateTime AssessmentDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string? Description { get; set; }

        public string? MaterialsNeeded { get; set; }
    }

    public record class CreateAssessmentDTO : AssessmentBaseDTO
    {
        [Required]
        public Guid SectionId { get; set; }

        public string? CreatedBy { get; set; }
    }

    public record class UpdateAssessmentDTO : AssessmentBaseDTO
    {
        [Required]
        public Guid AssessmentId { get; set; }

        [Required]
        public Guid SectionId { get; set; }

        public string? ModifiedBy { get; set; }
    }

    public record class GetAssessmentDTO : AssessmentBaseDTO
    {
        public Guid AssessmentId { get; set; }
        public Guid SectionId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }

        // Additional fields for UI display
        public string? SectionCode { get; set; }
        public string? SectionName { get; set; }
        public string? CourseName { get; set; }
        public string? TermName { get; set; }
    }

    public record class DuplicateAssessmentDTO
    {
        [Required]
        public Guid TargetSectionId { get; set; }

        public string? NewAssessmentName { get; set; }

        public decimal? NewWeightPercentage { get; set; }

        public DateTime? NewAssessmentDate { get; set; }

        public DateTime? NewDueDate { get; set; }

        public string? CreatedBy { get; set; }
    }
}
