using Microsoft.AspNetCore.Authorization;
using OrderingSystem.ViewModel;
using Microsoft.AspNetCore.Mvc;
using OrderingSystem.Data;

namespace OrderingSystem.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly DbHelper _db;
        private readonly IConfiguration _config;
        private readonly CustomerRepository _customerRepository;
        private readonly ProductRepository _productRepository;
        private readonly OrderRepository _orderRepository;

        public OrdersController(DbHelper db, IConfiguration config, 
            CustomerRepository customerRepository, 
            ProductRepository productRepository,
            OrderRepository orderRepository) 
        {
            _db = db; _config = config; 
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public IActionResult Create()
        {
            var orderViewModel = new OrderViewModel();
            var customers = _customerRepository.GetAll();
            orderViewModel.Customers = customers;
            var products = _productRepository.GetAll();
            orderViewModel.Products = products;
            return View(orderViewModel);
        }

        [HttpGet]
        public IActionResult GetPrice(int productId)
        {
            var product = _productRepository.GetById(productId);
            if (product ==null) return Json(new { success = false });
            return Json(new { success = true, price = Convert.ToDecimal(product.Price) });
        }

        [HttpPost]
        public IActionResult Create([FromBody] OrderCreateDto dto)
        {
            if (dto == null || dto.Items == null || dto.Items.Count == 0)
                return BadRequest("Invalid order payload.");

            var orderId = _orderRepository.Create(dto);
            if(orderId > 0)
            { 
                return Json(new { success = true, orderId });
            }
            else
            {
                return Json(new { success = false, error = "Error" });
            }
        }

        public IActionResult Details(int id)
        {
            var order = _orderRepository.GetById(id);
            return View(order);
        }
    }

    public class OrderCreateDto
    {
        public int CustomerId { get; set; }
        public List<OrderItemDto>? Items { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
