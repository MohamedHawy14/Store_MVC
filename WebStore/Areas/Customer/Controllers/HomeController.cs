using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Models.Models;
using DataAcess.Repository.IReository;
namespace WebStore.Areas.Customer.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            IEnumerable<Product> productList = _unitOfWork.product.GetAll(IncludeProperties: "Category");
            return View(productList);
        }
        public IActionResult Details(int Id)
        {

            var Product = _unitOfWork.product.Get(u => u.Id == Id, IncludeProperties: "Category");
            if (Product == null)
            {
                return NotFound(); 
            }

            return View(Product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
