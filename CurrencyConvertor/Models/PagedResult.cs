namespace CurrencyConvertor.Models
{
    public class PagedResult<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public T Items { get; set; }
    }

}
