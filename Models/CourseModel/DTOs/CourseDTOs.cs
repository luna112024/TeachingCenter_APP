using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.CourseModel.DTOs
{
    public abstract record class CourseBaseDTO
    {
        [Required]
        [StringLength(20)]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [StringLength(200)]
        [Display(Name = "Course Name (Chinese)")]
        public string? CourseNameChinese { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Description (Chinese)")]
        public string? DescriptionChinese { get; set; }

        [Required]
        [Display(Name = "Level ID")]
        public Guid LevelId { get; set; } // Change from string Level to Guid LevelId

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Duration (Weeks)")]
        public int DurationWeeks { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Hours Per Week")]
        public int HoursPerWeek { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Total Hours")]
        public int TotalHours { get; set; }

        [Range(1, int.MaxValue)]
        [Display(Name = "Max Students")]
        public int MaxStudents { get; set; } = 20;

        [Range(1, int.MaxValue)]
        [Display(Name = "Min Students")]
        public int MinStudents { get; set; } = 5;

        [StringLength(20)]
        [Display(Name = "Age Group")]
        public string AgeGroup { get; set; } = "All Ages"; // 'Kids', 'Teens', 'Adults', 'All Ages'

        [Display(Name = "Prerequisites")]
        public string? Prerequisites { get; set; }

        [Display(Name = "Learning Outcomes")]
        public string? LearningOutcomes { get; set; }

        [Display(Name = "Materials Included")]
        public string? MaterialsIncluded { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        [Display(Name = "Base Fee")]
        public decimal BaseFee { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [Display(Name = "Materials Fee")]
        public decimal MaterialsFee { get; set; } = 0.00m;

        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // 'Active', 'Inactive', 'Draft'
    }

    public record class CreateCourseDTO : CourseBaseDTO
    {
        public string? CreatedBy { get; set; }
    }

    public record class UpdateCourseDTO : CourseBaseDTO
    {
        public Guid CourseId { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetCourseDTO : CourseBaseDTO
    {
        public Guid CourseId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        // Add missing fields for UI display
        public string? LevelName { get; set; }
        public string? LevelCode { get; set; }
        public string? LevelNameChinese { get; set; }
    }

    public record class DuplicateCourseDTO
    {
        [Required]
        [StringLength(20)]
        [Display(Name = "New Course Code")]
        public string NewCourseCode { get; set; }
    }
}
