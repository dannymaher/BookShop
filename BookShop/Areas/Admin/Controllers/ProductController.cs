using BookShop.DataAccess.Repository;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Models.ViewModels;
using BookShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
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
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            
            return View(products);
        }
        //TODO - Create viewModel for index and remove some attributes from view
        public IActionResult Upsert(int? id)
        {
           
            //ViewBag.CategoryList = categoryList;
            //ViewData["CategoryList"] = categoryList;
            ProductVM ViewModel = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),
                Product = new Product()
            };
            if(id == null || id == 0)
            {
                //create functionality
                return View(ViewModel);
            }
            else
            {
                //edit/update functionality
                ViewModel.Product = _unitOfWork.Product.Get(q => q.Id == id);
                return View(ViewModel);
            }
        }
        [HttpPost]
        public IActionResult upsert(ProductVM ProductVM,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images/product");
                    if (!string.IsNullOrEmpty(ProductVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, ProductVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    ProductVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if(ProductVM.Product.Id == 00)
                {
                    _unitOfWork.Product.Add(ProductVM.Product);
                    TempData["success"] = ProductVM.Product.Title + " product created successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(ProductVM.Product);
                    TempData["success"] = ProductVM.Product.Title + " product updated successfully";
                }
                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
            else
            {


                ProductVM.CategoryList = _unitOfWork.Category.GetAll().ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
                
                return View(ProductVM);
            }
            
        }
       
        //public IActionResult Delete(int? Id)
        //{
        //    if (Id == null || Id == 0)
        //    {
        //        return NotFound();
        //    }

        //    Product? productFromDB = _unitOfWork.Product.Get(q => q.Id == Id);
        //    if (productFromDB == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDB);
        //}
        //[HttpPost,ActionName("Delete")]
        //public IActionResult DeletePOST(int? Id)
        //{
        //    Product? product = _unitOfWork.Product.Get(q => q.Id == Id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Remove(product);
        //    _unitOfWork.Save();
        //    TempData["success"] = product.Title + " product deleted successfully";
        //    return RedirectToAction("Index");
        //}
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted =  _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new {success = false, message = "Error while deleting"});
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
           
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

    }
}
