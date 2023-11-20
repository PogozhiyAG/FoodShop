using FoodShop.Web.Models;
using FoodShop.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodShop.Web.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IOrderCalculator _orderCalculator;

        public BasketController(IBasketService basketService, IOrderCalculator orderCalculator)
        {
            _basketService = basketService;
            _orderCalculator = orderCalculator;
        }

        public IActionResult Index()
        {
            var basketId = GetOrCreateBasketCookie();
            var basket = _basketService.GetBasket(basketId);
            var basketModel = _orderCalculator.Calculate(basket);
            return View(basketModel);
        }


        public IActionResult GetBasket()
        {
            var basketId = GetOrCreateBasketCookie();
            var basket = _basketService.GetBasket(basketId);
            var basketModel = _orderCalculator.Calculate(basket);
            return Ok(basketModel);
        }

        public IActionResult SetBasketItem(int productId, int quantity)
        {
            var basketId = GetOrCreateBasketCookie();
            _basketService.SetItem(basketId, productId, quantity);
            var basket = _basketService.GetBasket(basketId);
            var basketModel = _orderCalculator.Calculate(basket);
            return Ok(basketModel);
        }

        public IActionResult AddBasketItem(int productId, int quantity)
        {
            var basketId = GetOrCreateBasketCookie();
            _basketService.AddItem(basketId, productId, quantity);
            var basket = _basketService.GetBasket(basketId);
            var basketModel = _orderCalculator.Calculate(basket);
            return Ok(basketModel);
        }


        private string GetOrCreateBasketCookie()
        {
            if (User.Identity.IsAuthenticated)
            {
                return User.Identity.Name;
            }

            if (Request.Cookies.TryGetValue(Constatnts.BasketCookieName, out var cookieValue))
            {
                return cookieValue;
            }

            var basketKey = Guid.NewGuid().ToString();
            Response.Cookies.Append(
                Constatnts.BasketCookieName,
                basketKey,
                new CookieOptions() {
                    Expires = DateTime.Now.AddYears(10)
                }
            );

            return basketKey;
        }
    }
}
