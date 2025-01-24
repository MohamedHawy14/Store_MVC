using DataAcess.Repository.IReository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels;
using Stripe.Checkout;
using System.Security.Claims;
using utilities;

namespace WebStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork,IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            this._emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new() { shoppingCartlist = _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"), OrderHeader=new() };
            IEnumerable<ProductImage> productImages = _unitOfWork.productImage.GetAll();
            foreach (var cart in ShoppingCartVM.shoppingCartlist)
            {
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            
            ShoppingCartVM = new() { shoppingCartlist = _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"), OrderHeader = new() };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.applicationUser.Get(U => U.Id == userId);
            
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.shoppingCartlist)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
          
        }

        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM.shoppingCartlist = _unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
            
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.applicationUser.Get(U => U.Id == userId);

			foreach (var cart in ShoppingCartVM.shoppingCartlist)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //is a regular customer
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            }
            else
            {
                //is company
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusShipped;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            }
            _unitOfWork.orderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var cart in ShoppingCartVM.shoppingCartlist)
            {
                OrderDetial orderDetial = new() { ProductId=cart.ProductId,OrderHeaderId=ShoppingCartVM.OrderHeader.Id ,Price=cart.Price,Count=cart.Count};
				_unitOfWork.orderDetials.Add(orderDetial);
				_unitOfWork.Save();
			}
   

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //is a regular customer
                //Stripe logic
                var domain = "https://localhost:7269/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "Customer/Cart/Index",

                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                   
                    Mode = "payment",
                };

				foreach (var item in ShoppingCartVM.shoppingCartlist)
				{
					var sessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product.Title
							}
						},
						Quantity = item.Count
					};
					options.LineItems.Add(sessionLineItem);
				}
				var service = new SessionService();
				Session session = service.Create(options);
				_unitOfWork.orderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
				_unitOfWork.Save();
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}
			return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });

		}

		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == id, IncludeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.orderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.orderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.Clear();

            }
            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Hawy Store",
              $"<p>New Order Created - {orderHeader.Id}</p>");

            List<ShoppingCart> shoppingCarts = _unitOfWork.shoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.shoppingCart.DeleteRange(shoppingCarts);
            _unitOfWork.Save();




				return View(id);
		}

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.shoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                //remove that from cart

                _unitOfWork.shoppingCart.Delete(cartFromDb);
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.shoppingCart
                    .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.shoppingCart.Update(cartFromDb);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);

            _unitOfWork.shoppingCart.Delete(cartFromDb);

            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.shoppingCart
              .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
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
