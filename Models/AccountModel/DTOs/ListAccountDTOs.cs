namespace hongWenAPP.Models.AccountModel.DTOs
{
    public class ListAccountDTOs:PageGeneral
    {
        public PageList<GetAccountDTO> account { get; set; }
    }
}
