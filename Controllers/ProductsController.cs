using Microsoft.AspNetCore.Authorization;
using OrderingSystem.ViewModel;
using Microsoft.AspNetCore.Mvc;
using OrderingSystem.Models;

namespace OrderingSystem.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ProductRepository _productRepository;
        public ProductsController(ProductRepository productRepository)  { 
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadProducts(string search, int page = 1, int pageSize = 5)
        {
            var (products, totalCount) = _productRepository.GetProducts(search, page, pageSize);

            var viewModel = new ProductListViewModel
            {
                Products = products,
                SearchTerm = search,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return PartialView("_ProductsTable", viewModel);
        }

        public IActionResult Create()
        {
            return PartialView("_CreateEdit", new Product());
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Create(product);
                return Json(new { success = true });
            }
            return PartialView("_CreateEdit", product);
        }

        public IActionResult Details(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();
            return PartialView("_CreateEdit", product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Update(product);
                return Json(new { success = true });
            }
            return PartialView("_CreateEdit", product);
        }
    }
}
