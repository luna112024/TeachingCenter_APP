using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.TermModel.DTOs
{
    public abstract record class TermBaseDTO
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Term Name")]
        public string TermName { get; set; }
        
        [Required]
        [StringLength(20)]
        [Display(Name = "Term Code")]
        public string TermCode { get; set; }
        
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        
        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        
        [Required]
        [Display(Name = "Registration Start")]
        public DateTime RegistrationStart { get; set; }
        
        [Required]
        [Display(Name = "Registration End")]
        public DateTime RegistrationEnd { get; set; }
        
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";
        
        [Display(Name = "Is Current")]
        public bool Iscurrent { get; set; } = false;
        
        [Required]
        [StringLength(20)]
        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; }
    }
    
    public record class CreateTermDTO : TermBaseDTO
    {
        public string? CreatedBy { get; set; }
    }
    
    public record class UpdateTermDTO : TermBaseDTO
    {
        public Guid TermId { get; set; }
        public string? ModifiedBy { get; set; }
    }
    
    public record class GetTermDTO : TermBaseDTO
    {
        public Guid TermId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
