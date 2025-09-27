namespace hongWenAPP.Models
{
    public class PaginationModel
    {
        public string PartialAction {  get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItem { get; set; }
        public int RowOrder { get; set; }
        public int StartRowNo => (PageIndex - 1) * PageSize + 1;
        public int PreviousePageIndex => PageIndex - 1;
        public int NextPageIndex => (PageIndex + 1);
        // Cannot Back to -1 Page
        public bool HasPreviousePage => (PageIndex > 1);
        //Cannot Next Page 
        public bool HasNextPage => (PageIndex < TotalPages);
        public string HidePreviousePage => HasPreviousePage ? "" : "disabled";
        public string HideNextPage => HasNextPage ? "" : "disabled";
    }
}
