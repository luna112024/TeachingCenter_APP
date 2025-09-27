using hongWenAPP.Models.RolesModel.DTOs;
using System.ComponentModel.DataAnnotations;

namespace hongWenAPP.Models.PermissionModel.DTOs
{
    public abstract record class PermissionBaseDTO
    {
        [Required]
        [StringLength(100)]
        public string PermissionName { get; set; }
        [Required]
        [StringLength(50)]
        public string PermissionCode { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
    }

    // DTO for creating a new permission
    public record class CreatePermissionDTO : PermissionBaseDTO
    {
        public string? CreateBy { get; set; }
    }

    // DTO for updating an existing permission
    public record class UpdatePermissionDTO : PermissionBaseDTO
    {
        public Guid PermissionId { get; set; }
        public string? ModifyBy { get; set; }
    }

    // DTO for retrieving permission information
    public record class GetPermissionDTO : PermissionBaseDTO
    {
        public Guid PermissionId { get; set; }
        public string? CreateBy { get; set; }
        public string? ModifyBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<GetRoleDTO> Roles { get; set; } = new List<GetRoleDTO>();
    }
}
