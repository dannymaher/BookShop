using BookShop.DataAccess.Repository;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll().ToList();
            
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
                _unitOfWork.Product.Add(ProductVM.Product);
                _unitOfWork.Save();
                TempData["success"] = ProductVM.Product.Title + " product created successfully";
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
       
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            Product? productFromDB = _unitOfWork.Product.Get(q => q.Id == Id);
            if (productFromDB == null)
            {
                return NotFound();
            }
            return View(productFromDB);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int? Id)
        {
            Product? product = _unitOfWork.Product.Get(q => q.Id == Id);
            if (product == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = product.Title + " product deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
