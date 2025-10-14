namespace hongWenAPP.Models.GradeModel.DTOs
{
    public class ListGradeDTOs : PageGeneral
    {
        public PageList<GetGradeDTO> grade { get; set; }
    }
}
