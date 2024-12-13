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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.company.GetAll().ToList();

            return View(objCompanyList);
        }

        public IActionResult Upsert(int? id)
        {

            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company companyObj = _unitOfWork.company.Get(u => u.Id == id);
                return View(companyObj);
            }

        }
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {

                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.company.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.company.Update(CompanyObj);
                }

                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {

                return View(CompanyObj);
            }
        }
        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            var Company = _unitOfWork.company.Get(e => e.Id == id);
            if (Company == null)
                return NotFound();
            return View(Company);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id is null || id == 0)
            {
                return NotFound();
            }
            var Company = _unitOfWork.company.Get(e => e.Id == id);
            _unitOfWork.company.Delete(Company);
            _unitOfWork.Save();
            TempData["success"] = "Company Deleted Sucessfully";
            return RedirectToAction(nameof(Index));

        }


        #region Api Calls



        #endregion
    }
}
