namespace hongWenAPP.Models.FeeModel.DTOs
{
    public class ListFeeCategoryDTOs : PageGeneral
    {
        public PageList<GetFeeCategoryDTO> feeCategory { get; set; }
    }

    public class ListFeeTemplateDTOs : PageGeneral
    {
        public PageList<GetFeeTemplateDTO> feeTemplate { get; set; }
    }

    public class ListStudentFeeDTOs : PageGeneral
    {
        public PageList<GetStudentFeeDTO> studentFee { get; set; }
    }
}
