using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace FoodShop.Web.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            GetOrCreateBasketCookie();
            return View();
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
