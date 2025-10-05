namespace hongWenAPP.Models.AssessmentModel.DTOs
{
    public class ListAssessmentDTOs : PageGeneral
    {
        public PageList<GetAssessmentDTO> assessment { get; set; }
    }
}
