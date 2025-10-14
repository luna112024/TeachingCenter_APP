namespace hongWenAPP.Models.AttendanceModel.DTOs
{
    public class ListAttendanceDTOs : PageGeneral
    {
        public PageList<GetAttendanceDTO> attendance { get; set; }
    }
}