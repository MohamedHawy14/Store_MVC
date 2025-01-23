using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using WebStore.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels;

namespace WebStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext db , UserManager<IdentityUser> userManager)
        {
            _db = db;
            this._userManager = userManager;
        }

        public IActionResult Index()
        {
            List<ApplicationUser> users = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userroles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var item in users)
            {
                var userRole = userroles.FirstOrDefault(u => u.UserId == item.Id);
                if (userRole != null)
                {
                    var roleid = userRole.RoleId;
                    var role = roles.FirstOrDefault(u => u.Id == roleid);
                    item.Role = role?.Name ?? "No Role";
                }
                else
                {
                    item.Role = "No Role";
                }

                if (item.Company == null)
                {
                    item.Company = new Company() { Name = "" };
                }
            }

            return View(users);
        }

        [HttpPost]
       
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking the user." });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now; // Unlock
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000); // Lock
            }

            _db.ApplicationUsers.Update(objFromDb);
            _db.SaveChanges();

            return Json(new { success = true, message = "Operation successful." });
        }

        public IActionResult RoleManagment(string userId)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = _db.ApplicationUsers.Include(u=>u.Company).FirstOrDefault(u=>u.Id==userId),
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _db.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;
                    
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagmentVM.ApplicationUser.Id).RoleId;
            string OldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;



            if (!(roleManagmentVM.ApplicationUser.Role == OldRole))
            {

                //a role was updated
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u=>u.Id==roleManagmentVM.ApplicationUser.Id);
                if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                if (OldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _db.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUser, OldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }
           

            return RedirectToAction("Index");
        }

    }
}
