namespace hongWenAPP.Models.RolesModel.DTOs
{
    public class ListRoles :PageGeneral
    {
        public PageList<GetRoleDTO> roles { get; set; }
    }
}
