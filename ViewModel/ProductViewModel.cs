using OrderingSystem.Models;

namespace OrderingSystem.ViewModel
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new();
        public string? SearchTerm { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; } = 5;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
