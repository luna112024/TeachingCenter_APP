namespace hongWenAPP.Models.CompanyModel.DTOs
{
    public class ListCompanyDTOs:PageGeneral
    {
        public PageList<GetCompanyDTO> company { get; set; }
    }
}
