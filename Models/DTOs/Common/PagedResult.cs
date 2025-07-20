namespace Online_Learning.Models.DTOs.Common
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}
