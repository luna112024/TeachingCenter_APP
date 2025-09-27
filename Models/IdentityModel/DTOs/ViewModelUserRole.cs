using hongWenAPP.Models.RolesModel.DTOs;

namespace hongWenAPP.Models.IdentityModel.DTOs
{
    public class ViewModelUserRole : PageGeneral
    {
        public List<GetUsersDTOs> Users { get; set; } = new List<GetUsersDTOs>();
        public List<GetRoleDTO> Roles { get; set; } = new List<GetRoleDTO>();
    }
}
