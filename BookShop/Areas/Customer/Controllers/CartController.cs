using BookShop.DataAccess.Repository;
using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId,
                includeProperties: "Product"),
            };
            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = getPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Summary()
        {
            return View();
        }
        public IActionResult Plus(int cartID)
        {
            ShoppingCart CartFromDB = _unitOfWork.ShoppingCart.Get(u => u.Id == cartID);
            CartFromDB.Count += 1;
            _unitOfWork.ShoppingCart.Update(CartFromDB);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartID)
        {
            ShoppingCart CartFromDB = _unitOfWork.ShoppingCart.Get(u => u.Id == cartID);
            if(CartFromDB.Count <= 1) {
                Remove(cartID); return RedirectToAction(nameof(Index));
            }
            else
            {
                CartFromDB.Count -= 1;
                _unitOfWork.ShoppingCart.Update(CartFromDB);
            }
            
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartID)
        {
            ShoppingCart CartFromDB = _unitOfWork.ShoppingCart.Get(u => u.Id == cartID);
            
            _unitOfWork.ShoppingCart.Remove(CartFromDB);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        
        
        private double getPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            

            if(shoppingCart.Count <=50)
            {
                return shoppingCart.Product.Price;
            }
            else if(shoppingCart.Count >= 51 && shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
            
        }
    }
}
