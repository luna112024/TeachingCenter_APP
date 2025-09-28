namespace hongWenAPP.Models.ClassroomModel.DTOs
{
    public class ListClassroomDTOs : PageGeneral
    {
        public PageList<GetClassroomDTO> classroom { get; set; }
    }
}
