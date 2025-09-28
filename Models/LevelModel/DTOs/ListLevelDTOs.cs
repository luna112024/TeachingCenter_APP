namespace hongWenAPP.Models.LevelModel.DTOs
{
    public class ListLevelDTOs : PageGeneral
    {
        public PageList<GetLevelDTO> level { get; set; }
    }
}
