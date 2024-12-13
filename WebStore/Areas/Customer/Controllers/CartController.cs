using DataAcess.Repository.IReository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using System.Security.Claims;
using utilities;

namespace WebStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new() { shoppingCartlist = _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product") };
            foreach (var cart in ShoppingCartVM.shoppingCartlist)
            {
                 cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb != null)
            {
                cartFromDb.Count += 1;
                _unitOfWork.shoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
                TempData["success"] = "Item quantity updated successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb != null)
            {
                if (cartFromDb.Count > 1)
                {
                    cartFromDb.Count -= 1;
                    _unitOfWork.shoppingCart.Update(cartFromDb);
                }
                else
                {
                    _unitOfWork.shoppingCart.Delete(cartFromDb);
                }
                _unitOfWork.Save();
                TempData["success"] = "Item quantity updated successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb != null)
            {
                _unitOfWork.shoppingCart.Delete(cartFromDb);
                _unitOfWork.Save();
                TempData["success"] = "Item removed from the cart.";
            }
            return RedirectToAction(nameof(Index));
        } 
        public IActionResult Summary()
        {
            return View();
        }



        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
