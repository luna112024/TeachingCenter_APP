namespace hongWenAPP.Models.StudentModel.DTOs
{
    public class ListStudentDTOs : PageGeneral
    {
        public PageList<GetStudentDTO> student { get; set; }
    }
}
