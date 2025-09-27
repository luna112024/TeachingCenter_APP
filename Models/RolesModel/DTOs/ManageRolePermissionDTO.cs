using hongWenAPP.Models.PermissionModel.DTOs;
using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.RolesModel.DTOs
{
    public class ManageRolePermissionDTO
    {
        [Required]
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public List<GetPermissionDTO> AvailablePermissions { get; set; } = new List<GetPermissionDTO>();
        public List<GetPermissionDTO> AssignedPermissions { get; set; } = new List<GetPermissionDTO>();
    }

    public class RolePermissionAssignmentDTO
    {
        [Required]
        public Guid RoleId { get; set; }
        [Required]
        public List<Guid> PermissionIds { get; set; } = new List<Guid>();
    }
} 