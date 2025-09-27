using hongWenAPP.Models.CompanyModel.DTOs;
using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.AccountModel.DTOs
{
    public abstract record class AccountBaseDTOs
    { 
        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; }
        [Required]
        public Guid CompanyId { get; set; }
        public bool IsActive { get; set; }

        public List<GetCompanyDTO>? Companies { get; set; }
        public string? Currency { get; set; }
    }
    public record class CreateAccountDTO : AccountBaseDTOs
    {
        public string? CreateBy { get; set; }
    }
    public record class UpdateAccountDTO : AccountBaseDTOs
    {
        public Guid AccID { get; set; }
        public string? ModifyBy { get; set; }
    }
    public record class GetAccountDTO : AccountBaseDTOs
    {
        public Guid AccID { get; set; }
        public string? CreateBy { get; set; }
        public string? ModifyBy { get; set; }
        public string? CompanyName { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
       
    }
}
