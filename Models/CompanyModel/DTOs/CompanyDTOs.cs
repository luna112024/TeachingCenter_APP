using hongWenAPP.Models.IdentityModel.DTOs;
using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.CompanyModel.DTOs
{
    public abstract record class CompanyBaseDTO
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Company Name")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Company Name can only contain letters, numbers and spaces")]
        public string CompanyName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Description can only contain letters, numbers and spaces")]
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
    }

    public record class CreateCompanyDTO : CompanyBaseDTO
    {
        public string? CreateBy { get; set; }
    }
    public record class UpdateCompanyDTO : CompanyBaseDTO
    {
        public Guid CompanyId { get; set; }
        public string? ModifyBy { get; set; }
    }
    public record class GetCompanyDTO : CompanyBaseDTO
    {
        public Guid CompanyId { get; set; }
        public string? CreateBy { get; set; }
        public string? ModifyBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<GetUsersDTOs> Users { get; set; } = new List<GetUsersDTOs>();
    }
}
