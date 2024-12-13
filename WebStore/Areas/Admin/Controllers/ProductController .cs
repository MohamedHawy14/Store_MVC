using Microsoft.AspNetCore.Mvc;
using Models.Models;
using DataAcess.Repository.IReository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using utilities;

namespace WebStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public  IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.product.GetAll(includeProperties: "Category").ToList();

            return View(objProductList);
        }
        public IActionResult UpSert(int? id) //Update & Create
        {
            var ProductVM = new ProductVM() 
            { 
                CategoryList = _unitOfWork.category.GetAll().ToList().Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() })
                , Product=new Product() 
            };
            if(id==null || id == 0)
            {
                //Create
                return View(ProductVM);
            }
            else
            {
                //Update
                ProductVM.Product = _unitOfWork.product.Get(u => u.Id == id);
                return View(ProductVM);
            }

           
        }
        [HttpPost]
        public IActionResult UpSert(ProductVM productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                #region ProductImage
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string ProductPath = Path.Combine(wwwRootPath, @"Images\Product");
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //Delete Old Image
                        var oldimagepath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldimagepath))
                        {
                            System.IO.File.Delete(oldimagepath);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(ProductPath, FileName), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.ImageUrl = @"\Images\Product\" + FileName;

                }
                #endregion
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product Created Sucessfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {


                productVM.CategoryList = _unitOfWork.category.GetAll().ToList().Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });


                return View(productVM);
            }

        }



        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            var Product = _unitOfWork.product.Get(e => e.Id == id);
            if (Product == null)
                return NotFound();
            return View(Product);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            var Product = _unitOfWork.product.Get(e => e.Id == id);
            _unitOfWork.product.Delete(Product);
            _unitOfWork.Save();
            TempData["success"] = "Product Deleted Sucessfully";
            return RedirectToAction(nameof(Index));

        }

        #region Api Calls
      


        #endregion
    }
}
