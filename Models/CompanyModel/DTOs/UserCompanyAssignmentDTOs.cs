using hongWenAPP.Models.IdentityModel.DTOs;
using hongWenAPP.Models.CompanyModel.DTOs;
using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.CompanyModel.DTOs
{
    public class ManageUserCompanyDTO
    {
        [Required]
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<GetCompanyDTO> AvailableCompanies { get; set; } = new List<GetCompanyDTO>();
        public List<GetCompanyDTO> AssignedCompanies { get; set; } = new List<GetCompanyDTO>();
    }

    public class UserCompanyAssignmentDTO
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public List<Guid> CompanyIds { get; set; } = new List<Guid>();
    }

    public class UserCompanyAssignDto
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
    }
} 