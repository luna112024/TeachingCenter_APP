namespace hongWenAPP.Models.TeacherModel.DTOs
{
    public class ListTeacherDTOs : PageGeneral
    {
        public PageList<GetTeacherDTO> teacher { get; set; }
    }
}