using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodShop.Api.Catalog.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly FoodShopDbContext _db;

        public CatalogController(FoodShopDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            return _db.Products.Take(10);
        }
    }
}
