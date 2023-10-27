using FoodShop.Infrastructure.Data;
using FoodShop.Web.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics;

namespace FoodShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FoodShopDbContext  _dbContext;

        public HomeController(ILogger<HomeController> logger, FoodShopDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = new HomeIndexModel();
            model.ProductCategories = _dbContext.ProductCategories.ToList();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public string Test()
        {
            var connection =  ConnectionMultiplexer.Connect("basketdata");
            var cache = connection.GetDatabase();
            cache.StringSet("key1", DateTime.Now.ToLongTimeString());
            return "OK";
        }

        public string Test2()
        {
            var connection = ConnectionMultiplexer.Connect("basketdata");
            var cache = connection.GetDatabase();
            var result = cache.StringGet("key1");
            return result;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}