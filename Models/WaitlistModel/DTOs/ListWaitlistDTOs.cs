namespace hongWenAPP.Models.WaitlistModel.DTOs
{
    public class ListWaitlistDTOs : PageGeneral
    {
        public PageList<GetWaitlistDTO> waitlist { get; set; }
    }
}
