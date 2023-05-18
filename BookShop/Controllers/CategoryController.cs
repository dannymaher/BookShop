﻿using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }

        public IActionResult Create() {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
           /* if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The display order cannot match the name");
            }*/
            /*if (obj.Name != null && obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is an invalid value");
            }*/
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = obj.Name + " category created successfully";
                return RedirectToAction("Index");
            }
            return View();
           
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id==0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
           
            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();
                TempData["success"] = obj.Name + " category updated successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
            
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST( int? id)
        {
            Category? obj = _db.Categories.FirstOrDefault(c => c.Id == id);
            if(obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = obj.Name + " category deleted successfully";
            return RedirectToAction("Index");
            
        }
    }
}
