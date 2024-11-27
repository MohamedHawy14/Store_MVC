using Microsoft.AspNetCore.Mvc;
using WebStore.Data.Contexts;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            var Categories = _context.Categories.ToList();
            return View(Categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if(category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Name $ Display Order Musn't Be Same Value");
            }
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["success"] = "Category Created Sucessfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public IActionResult Edit(int? id)
        {
            if(id is null || id == 0)
            {
                return NotFound();
            }
            var Category = _context.Categories.Find(id);
            if (Category == null)
                return NotFound();
            return View(Category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category is null || category.Name == null) return NotFound();
            _context.Categories.Update(category);
            _context.SaveChanges();
            TempData["success"] = "Category Updated Sucessfully";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            var Category = _context.Categories.Find(id);
            if (Category == null)
                return NotFound();
            return View(Category);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            var category = _context.Categories.Find(id);
            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["success"] = "Category Deleted Sucessfully";
            return RedirectToAction(nameof(Index));

        }
    }
}
