namespace hongWenAPP.Models
{
    public class PageGeneral
    {
        public PageGeneral()
        {
            Page = 1;
            PageSize = 10;
            SearchText = "";
        }
        public Guid Id1 { get; set; }
        public Guid Id2 { get; set; }
        public Guid Id3 { get; set; }


        public string SearchText { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
