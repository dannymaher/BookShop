using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers
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
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            
            return View(companies);
        }

        public IActionResult Upsert(int? id) {
            Company company = new Company();
            if(id == null || id == 0)
            {
                return View(company);
            }
            else
            {
                 company = _unitOfWork.Company.Get(i => i.Id == id);
                

                return View(company);
            }

            
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if(company.Id == 0)
            {
                if(ModelState.IsValid)
                {
                    _unitOfWork.Company.Add(company);
                    _unitOfWork.Save();
                    TempData["success"] = company.Name + " company created successfully";
                    return (RedirectToAction("Index"));
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.Company.Update(company);
                    _unitOfWork.Save();
                    TempData["success"] = company.Name + " company updated successfully";
                    return (RedirectToAction("Index"));
                }
            }
            return View();
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Companies = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = Companies });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Company CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return(NotFound());
            }
            else
            {
                _unitOfWork.Company.Remove(CompanyToBeDeleted);
                _unitOfWork.Save();
            }
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
