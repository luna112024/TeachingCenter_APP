using hongWenAPP.Models.PermissionModel.DTOs;
using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.RolesModel.DTOs
{
    // Base DTO with common properties
    public abstract record class RoleBaseDTO
    {
        [Required]
        [StringLength(50)]

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public string Description { get; set; }
    }

    // DTO for creating a new role
    public record class CreateRoleDTO : RoleBaseDTO
    {
        public string CreateBy { get; set; } = "";
    }

    // DTO for updating an existing role
    public record class UpdateRoleDTO : RoleBaseDTO
    {
        public Guid RoleId { get; set; }
        public string ModifyBy { get; set; } = "";
    }

    // DTO for retrieving role information
    public record class GetRoleDTO : RoleBaseDTO
    {
        public Guid RoleId { get; set; }
        public string CreateBy { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<GetPermissionDTO> Permissions { get; set; } = new List<GetPermissionDTO>();
    }
}
