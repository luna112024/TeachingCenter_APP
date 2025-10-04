namespace hongWenAPP.Models.EnrollmentModel.DTOs
{
    public class ListEnrollmentDTOs : PageGeneral
    {
        public PageList<GetEnrollmentDTO> enrollment { get; set; }
    }
}
