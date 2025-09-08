using OrderingSystem.Models;

namespace OrderingSystem.ViewModel
{
    public class OrderViewModel
    {
        public List<Customer> Customers { get; set; }
        public List<Product> Products { get; set; }

        public OrderViewModel()
        {
            Customers = new List<Customer>();
            Products = new List<Product>();
        }
    }
}
