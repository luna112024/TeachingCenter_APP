using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.LevelModel.DTOs
{
    public abstract record class LevelBaseDTO
    {
        [Required]
        [StringLength(10)]
        [Display(Name = "Level Code")]
        public string LevelCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Level Name")]
        public string LevelName { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Level Name (Chinese)")]
        public string? LevelNameChinese { get; set; }

        [StringLength(50)]
        [Display(Name = "Level Name (Khmer)")]
        public string? LevelNameKhmer { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Level Order")]
        public int LevelOrder { get; set; }

        [Display(Name = "Parent Level ID")]
        public Guid? ParentLevelId { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Description (Chinese)")]
        public string? DescriptionChinese { get; set; }

        [Display(Name = "Prerequisites")]
        public string? Prerequisites { get; set; }

        [Display(Name = "Learning Objectives")]
        public string? LearningObjectives { get; set; }

        [Range(1, int.MaxValue)]
        [Display(Name = "Expected Hours")]
        public int? ExpectedHours { get; set; }

        [StringLength(10)]
        [Display(Name = "HSK Level")]
        public string? HskLevel { get; set; }

        [StringLength(5)]
        [Display(Name = "CEFR Equivalent")]
        public string? CefrEquivalent { get; set; }

        [StringLength(20)]
        [Display(Name = "Min Age Group")]
        public string? MinAgeGroup { get; set; }

        [StringLength(20)]
        [Display(Name = "Max Age Group")]
        public string? MaxAgeGroup { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Is For Beginner")]
        public bool IsForBeginner { get; set; } = false;

        [Display(Name = "Is For Placement")]
        public bool IsForPlacement { get; set; } = true;
    }

    public record class CreateLevelDTO : LevelBaseDTO
    {
        public string? CreatedBy { get; set; }
    }

    public record class UpdateLevelDTO : LevelBaseDTO
    {
        public Guid LevelId { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public record class GetLevelDTO : LevelBaseDTO
    {
        public Guid LevelId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        // Additional fields for UI display
        public string? ParentLevelName { get; set; }
        public int SubLevelsCount { get; set; }
        public int CoursesCount { get; set; }
        public int StudentsCount { get; set; }
    }

    public record class LevelListDTO
    {
        public Guid LevelId { get; set; }
        public string LevelCode { get; set; } = string.Empty;
        public string LevelName { get; set; } = string.Empty;
        public string? LevelNameChinese { get; set; }
        public string? LevelNameKhmer { get; set; }
        public int LevelOrder { get; set; }
        public string? ParentLevelName { get; set; }
        public string? HskLevel { get; set; }
        public string? CefrEquivalent { get; set; }
        public bool IsActive { get; set; }
        public bool IsForBeginner { get; set; }
        public bool IsForPlacement { get; set; }
    }

    public record class LevelHierarchyDTO
    {
        public Guid LevelId { get; set; }
        public string LevelCode { get; set; } = string.Empty;
        public string LevelName { get; set; } = string.Empty;
        public string? LevelNameChinese { get; set; }
        public int LevelOrder { get; set; }
        public Guid? ParentLevelId { get; set; }
        public bool IsActive { get; set; }
        public List<LevelHierarchyDTO> SubLevels { get; set; } = new List<LevelHierarchyDTO>();
    }
}
