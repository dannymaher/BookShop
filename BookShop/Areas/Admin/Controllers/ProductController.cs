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
                ViewModel.Product = _unitOfWork.Product.Get(q => q.Id == id, includeProperties:"ProductImages");
                return View(ViewModel);
            }
        }
        [HttpPost]
        public IActionResult upsert(ProductVM ProductVM,List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (ProductVM.Product.Id == 00)
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
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(files != null)
                {
                    foreach(IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + ProductVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath,productPath);

                        if(!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = ProductVM.Product.Id,
                        };
                        if(ProductVM.Product.ProductImages == null)
                        {
                            ProductVM.Product.ProductImages = new List<ProductImage>();
                            
                        }
                        ProductVM.Product.ProductImages.Add(productImage);
                        //_unitOfWork.ProductImage.Add(productImage);
                    }

                    _unitOfWork.Product.Update(ProductVM.Product);
                    _unitOfWork.Save();
                   
                }
               
                
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

        public IActionResult DeleteImage(int imageId)
        {
            var ImageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int ProductId = ImageToBeDeleted.ProductId;
            if (ImageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(ImageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, ImageToBeDeleted.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Remove(ImageToBeDeleted);
                _unitOfWork.Save();
                TempData["success"] = "Deleted successfully";

            }
            return RedirectToAction(nameof(upsert), new {id = ProductId});
        }
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
            

            
            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filepaths = Directory.GetFiles(finalPath);
                foreach (string filepath in filepaths)
                {
                    System.IO.File.Delete(filepath);
                }
                
                Directory.Delete(finalPath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
           
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

    }
}
