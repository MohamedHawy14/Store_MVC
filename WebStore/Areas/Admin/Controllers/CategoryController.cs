using Microsoft.AspNetCore.Mvc;
using Models.Models;
using DataAcess.Repository.IReository;
using Microsoft.AspNetCore.Authorization;
using utilities;

namespace WebStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var Categories = _unitOfWork.category.GetAll().ToList();
            return View(Categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Name $ Display Order Musn't Be Same Value");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category Created Sucessfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            var Category = _unitOfWork.category.Get(e => e.Id == id);
            if (Category == null)
                return NotFound();
            return View(Category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category is null || category.Name == null) return NotFound();
            _unitOfWork.category.Update(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Updated Sucessfully";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            var Category = _unitOfWork.category.Get(e => e.Id == id);
            if (Category == null)
                return NotFound();
            return View(Category);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            var category = _unitOfWork.category.Get(e => e.Id == id);
            _unitOfWork.category.Delete(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Sucessfully";
            return RedirectToAction(nameof(Index));

        }
    }
}
