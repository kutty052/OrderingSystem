using System.Collections.Generic;

namespace OrderingSystem.Models
{
    public class Order
    {
        public Order()
        {
            Items = new List<OrderItem>();
        }
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItem>? Items { get; set; }
    }
}
