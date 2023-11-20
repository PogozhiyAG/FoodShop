using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using FoodShop.Web.Models;
using FoodShop.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;
using NuGet.Common;
using StackExchange.Redis;
using System.Diagnostics;

namespace FoodShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FoodShopDbContext _dbContext;
        private readonly IProductPriceStrategyProvider _productPriceStrategyProvider;
        private readonly IUserTokenProvider _userTokenProvider;
        private readonly IMemoryCache _memoryCache;
        public HomeController(ILogger<HomeController> logger, FoodShopDbContext dbContext, IProductPriceStrategyProvider productPriceStrategyProvider, IUserTokenProvider userTokenProvider, IMemoryCache memoryCache)
        {
            _logger = logger;
            _dbContext = dbContext;
            _productPriceStrategyProvider = productPriceStrategyProvider;
            _userTokenProvider = userTokenProvider;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            var model = new HomeIndexModel();

            var tokenTypeIds = _userTokenProvider.GetUserTokens()
                .Select(x => x.TokenTypeId)
                .Concat(new[] { 0 })
                .Distinct()
                .ToList();

            model.ProductCategories = GetPopularCategories();
            model.ProductCards = GetProductsTest(0, 55);// GetPopularProducts(0, 30);

            var sw = Stopwatch.StartNew();
            foreach (var productCard in model.ProductCards)
            {
                productCard.Offers = _productPriceStrategyProvider.GetStrategyLinks(productCard.Product, tokenTypeIds);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public string Test()
        {
            var connection = ConnectionMultiplexer.Connect("basketdata");
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



        private IEnumerable<ProductCategoryModel> GetPopularCategories()
        {
            var key = $"{this.GetType()}.{nameof(GetPopularCategories)}";
            if (_memoryCache.TryGetValue(key, out IEnumerable<ProductCategoryModel>? result))
            {
                return result!;
            }

            result = _dbContext.ProductCategories
                .Where(c => c.ParentCategory == null)
                .Take(12)
                .Select(c => new ProductCategoryModel() { ProductCategory = c })
                .ToList();

            _memoryCache.Set(key, result, TimeSpan.FromHours(1));
            return result;
        }

        private IEnumerable<ProductCardModel> GetPopularProducts(int skip, int take)
        {
            var key = $"{this.GetType()}.{nameof(GetPopularProducts)}_{skip}_{take}";
            if (_memoryCache.TryGetValue(key, out IEnumerable<ProductCardModel>? result))
            {
                return result!;
            }

            result = _dbContext.Products.AsNoTracking()
                .Include(p => p.Tags)
                    .ThenInclude(t => t.Tag)
                .OrderByDescending(p => p.Popularity)
                .Skip(skip)
                .Take(take)
                .Select(p => new ProductCardModel()
                {
                    Product = p
                })
                .ToList();

            _memoryCache.Set(key, result, TimeSpan.FromMinutes(1));
            return result;
        }


        private IEnumerable<ProductCardModel> GetProductsTest(int skip, int take)
        {
            return _dbContext.Products.AsNoTracking()
                .Include(p => p.Tags)
                    .ThenInclude(t => t.Tag)
                .OrderByDescending(p => p.Popularity)
                .Skip(skip)
                .Take(take)
                .Select(p => new ProductCardModel()
                {
                    Product = p
                })
                .ToList();
        }
    }
}