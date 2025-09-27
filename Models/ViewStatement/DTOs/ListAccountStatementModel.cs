namespace hongWenAPP.Models.ViewStatement.DTOs
{
    public class ListAccountStatementModel : PageGeneral
    {
        public PageList<AccountStatementModel> transactions { get; set; }
    }
}
