using BookShop.DataAccess.Data;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            
            var productFromDb = _db.Products.FirstOrDefault(u => u.Id == product.Id);
            if(productFromDb != null)
            {
                productFromDb.Title = product.Title;
                productFromDb.ISBN = product.ISBN;
                productFromDb.Price = product.Price;
                productFromDb.Price50 = product.Price50;
                productFromDb.Price100 = product.Price100;
                productFromDb.ListPrice = product.ListPrice;
                productFromDb.Description = product.Description;
                productFromDb.Author = product.Author;
                productFromDb.CategoryId = product.CategoryId;
                if(product.ImageUrl != null )
                {
                    productFromDb.ImageUrl = product.ImageUrl;
                }
                
            }
        }
    }
}
