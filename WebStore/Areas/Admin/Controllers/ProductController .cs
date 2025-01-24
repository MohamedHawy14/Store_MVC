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


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.product.GetAll(includeProperties: "Category").ToList();

            return View(objProductList);
        }
        public IActionResult UpSert(int? id) //Update & Create
        {
            var ProductVM = new ProductVM()
            {
                CategoryList = _unitOfWork.category.GetAll().ToList().Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() })
                ,
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //Create
                return View(ProductVM);
            }
            else
            {
                //Update
                ProductVM.Product = _unitOfWork.product.Get(u => u.Id == id,IncludeProperties: "ProductImages");
                return View(ProductVM);
            }


        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.product.Update(productVM.Product);
                }

                _unitOfWork.Save();


                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);

                    }

                    _unitOfWork.product.Update(productVM.Product);
                    _unitOfWork.Save();




                }


                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
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

            // Fetch the product from the database
            var product = _unitOfWork.product.Get(e => e.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // Construct the path to the product's directory
            string productPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", $"product-{id}");

            try
            {
                // Check if the directory exists
                if (Directory.Exists(productPath))
                {
                    // Delete all files inside the directory
                    foreach (var filePath in Directory.GetFiles(productPath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    // Delete the directory itself
                    Directory.Delete(productPath);
                }

                // Remove the product from the database
                _unitOfWork.product.Delete(product);
                _unitOfWork.Save();

                // Success message
                TempData["success"] = "Product deleted successfully.";
            }
            catch (Exception ex)
            {
                // Log the error (use a logging framework like Serilog in a real application)
                Console.WriteLine($"An error occurred while deleting the product or directory: {ex.Message}");

                // Display error message to the user
                TempData["error"] = "An error occurred while deleting the product. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

       
        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.productImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                                   Path.Combine(_webHostEnvironment.WebRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.productImage.Delete(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }


    }
}

