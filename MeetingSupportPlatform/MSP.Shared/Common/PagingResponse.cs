namespace MSP.Shared.Common
{
    /// <summary>
    /// PagingResponse
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int TotalItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PagingResponse() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public PagingResponse(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
        {
            Items = items;
            TotalItems = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
