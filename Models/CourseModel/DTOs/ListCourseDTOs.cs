namespace hongWenAPP.Models.CourseModel.DTOs
{
    public class ListCourseDTOs : PageGeneral
    {
        public PageList<GetCourseDTO> course { get; set; }
    }
}
