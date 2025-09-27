using hongWenAPP.Models.IdentityModel.DTOs;
using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.RolesModel.DTOs
{
    public class ManageUserRoleDTO
    {
        [Required]
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<GetRoleDTO> AvailableRoles { get; set; } = new List<GetRoleDTO>();
        public List<GetRoleDTO> AssignedRoles { get; set; } = new List<GetRoleDTO>();
    }

    public class UserRoleAssignmentDTO
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }

    public class UserRoleAssignDto
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
} 