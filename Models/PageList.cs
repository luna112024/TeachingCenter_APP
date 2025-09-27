namespace hongWenAPP.Models
{
    public class PageList<T> : List<T>
    {

        public PaginationModel Paging { get; set; }


        public PageList(List<T> items, int count, int pageIndex, int pageSize, string partialAction)
        {
            Paging = new PaginationModel();
            Paging.PartialAction = partialAction;
            Paging.PageSize = pageSize;
            Paging.TotalItem = count;
            Paging.PageIndex = pageIndex;
            Paging.TotalPages = (int)Math.Ceiling(count / (decimal)pageSize);
            Paging.RowOrder = (pageSize * (Paging.PageIndex - 1)) + 1; //Ex: (10x(1-1))+1 = 1, (10x(2-1))+1 = 11           
            this.AddRange(items);
        }

        public static PageList<T> Create(List<T> fullData, int pageIndex, int pageSize, string partialAction)
        {
            var data = fullData.Count();
            var items = fullData.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return new PageList<T>(items, data, pageIndex, pageSize, partialAction);
        }
    }
}
