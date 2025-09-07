using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderingSystem.Data;

namespace OrderingSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly OrderRepository _orderRepository;
        public DashboardController(OrderRepository orderRepository) 
        {
            _orderRepository = orderRepository; 
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Stats(DateTime? from, DateTime? to)
        {
            var stats = _orderRepository.GetSalesStats(from ?? DateTime.Now.AddDays(-7), to ?? DateTime.Now);
            return Json(stats);
        }
    }
}
