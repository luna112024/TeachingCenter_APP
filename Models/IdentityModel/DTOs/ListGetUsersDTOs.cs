namespace hongWenAPP.Models.IdentityModel.DTOs
{
    public class ListGetUsersDTOs:PageGeneral
    {
        public PageList<GetUsersDTOs> users { get; set; }
    }
}
