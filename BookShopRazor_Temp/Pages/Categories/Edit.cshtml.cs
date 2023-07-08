using BookShopRazor_Temp.Data;
using BookShopRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookShopRazor_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Category Category { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        
        
       
        public void OnGet(int? id)
        {
            
            if(id != null && id != 0 )
            {
                Category = _db.Categories.FirstOrDefault(c => c.Id == id);
            }
          
        }
        public IActionResult OnPost() {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(Category);
                _db.SaveChanges();
                TempData["success"] = Category.Name + " category updated successfully";
                return (RedirectToPage("Index"));
            }
            return (Page());
            
        }
    }
}
