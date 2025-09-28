using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hongWenAPP.Models.ClassSectionModel.DTOs
{
    public abstract record class ClassSectionBaseDTO
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public Guid TermId { get; set; }

        [Required]
        public Guid TeacherId { get; set; }

        [Required]
        public Guid ClassroomId { get; set; }

        [Required]
        [StringLength(30)]
        public string SectionCode { get; set; }

        [StringLength(100)]
        public string? SectionName { get; set; }

        [Required]
        [CompareDates]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string SchedulePattern { get; set; } // JSON string for schedule

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxEnrollment { get; set; }

        [Range(0, int.MaxValue)]
        public int CurrentEnrollment { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int WaitlistCount { get; set; } = 0;

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal TuitionFee { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal MaterialsFee { get; set; } = 0.00m;

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal RegistrationFee { get; set; } = 0.00m;

        [StringLength(20)]
        public string Status { get; set; } = "Planning"; // 'Planning', 'Open', 'Full', 'Running', 'Completed', 'Cancelled'

        public string? Notes { get; set; }
    }

    public record class CreateClassSectionDTO : ClassSectionBaseDTO
    {
        public string? CreatedBy { get; set; }
    }

    public record class UpdateClassSectionDTO : ClassSectionBaseDTO
    {
        [Required]
        public Guid SectionId { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetClassSectionDTO : ClassSectionBaseDTO
    {
        public Guid SectionId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Navigation properties
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public string? TermName { get; set; }
        public string? TeacherName { get; set; }
        public string? ClassroomName { get; set; }
    }

    public record class DuplicateClassSectionDTO
    {
        [Required]
        [StringLength(30)]
        public string NewSectionCode { get; set; }

        [Required]
        public Guid NewTermId { get; set; }
    }

    public record class UpdateSectionStatusDTO
    {
        [Required]
        public string Status { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class ListClassSectionDTOs
    {
        public PageList<GetClassSectionDTO>? classSection { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchText { get; set; }
        public string? StatusFilter { get; set; }
        public Guid? CourseFilter { get; set; }
        public Guid? TermFilter { get; set; }
        public Guid? TeacherFilter { get; set; }
    }

    // Custom validation attribute for date comparison
    public class CompareDatesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime startDate)
            {
                // Get the EndDate property from the same object
                var endDateProperty = validationContext.ObjectType.GetProperty("EndDate");
                if (endDateProperty != null)
                {
                    var endDate = (DateTime)endDateProperty.GetValue(validationContext.ObjectInstance);
                    
                    // If both dates are set and start date is after end date, it's invalid
                    if (startDate > endDate)
                    {
                        return new ValidationResult("Start date must be before or equal to end date.");
                    }
                }
            }
            
            return ValidationResult.Success;
        }
    }
}
