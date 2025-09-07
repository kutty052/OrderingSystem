using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderingSystem.ViewModel;
using OrderingSystem.Models;

namespace OrderingSystem.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly CustomerRepository _customerRepository;

        public CustomersController(CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadCustomers(string search, int page = 1, int pageSize = 5)
        {
            var (customers, totalCount) = _customerRepository.GetCustomers(search, page, pageSize);

            var viewModel = new CustomerListViewModel
            {
                Customers = customers,
                SearchTerm = search,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return PartialView("_CustomersTable", viewModel);
        }

        public IActionResult Create()
        {
            return PartialView("_CreateEdit", new Customer());
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _customerRepository.Create(customer);
                return Json(new { success = true });
            }
            return PartialView("_CreateEdit", customer);
        }

        public IActionResult Details(int id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        public IActionResult Edit(int id)
        {
            var customer = _customerRepository.GetById(id);
            if (customer == null) return NotFound();
            return PartialView("_CreateEdit", customer);
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _customerRepository.Update(customer);
                return Json(new { success = true });
            }
            return PartialView("_CreateEdit", customer);
        }
    }
}
