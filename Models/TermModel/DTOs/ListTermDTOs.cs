namespace hongWenAPP.Models.TermModel.DTOs
{
    public class ListTermDTOs : PageGeneral
    {
        public PageList<GetTermDTO> term { get; set; }
    }
}
