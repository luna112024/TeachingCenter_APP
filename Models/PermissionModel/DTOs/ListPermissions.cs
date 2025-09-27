namespace hongWenAPP.Models.PermissionModel.DTOs
{
    public class ListPermissions:PageGeneral
    {
        public PageList<GetPermissionDTO> permissions { get; set; }
    }
}
