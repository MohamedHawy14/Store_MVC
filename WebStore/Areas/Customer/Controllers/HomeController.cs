using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Models.Models;
using DataAcess.Repository.IReository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using utilities;
namespace WebStore.Areas.Customer.Controllers
{
    [Area("Customer")]
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
            

            IEnumerable<Product> productList = _unitOfWork.product.GetAll(includeProperties: "Category,ProductImages");
            return View(productList);
        }
        public IActionResult Details(int Id)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.product.Get(u => u.Id == Id, IncludeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId=Id
            };
            

            
            if (shoppingCart == null)
            {
                return NotFound(); 
            }

            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.shoppingCart.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.shoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                //add cart record
                shoppingCart.Id = 0;
                _unitOfWork.shoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
              _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart updated successfully";




            return RedirectToAction(nameof(Index));
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
