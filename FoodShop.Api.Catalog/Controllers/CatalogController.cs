using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult GetProducts(
            int? categoryId,
            int? brandId,
            int? tagId,
            string? text,
            int? skip,
            int? take,
            ProductSortType? sort
        )
        {
            var q = _db.Products.AsNoTracking();

            if (!String.IsNullOrEmpty(text))
            {
                q = q.Where(p => EF.Functions.Contains(p.Name, text));
            }

            if (tagId.HasValue)
            {
                q = q.Where(p => p.Tags.Any(tr => tr.TagId == tagId));
            }
            if (categoryId.HasValue)
            {
                q = q.Where(p => p.CategoryId == categoryId.Value);
            }
            if (brandId.HasValue)
            {
                q = q.Where(p => p.BrandId == brandId.Value);
            }

            if (!sort.HasValue)
            {
                sort = ProductSortType.Popularity;
            }
            switch (sort)
            {
                case ProductSortType.Popularity: q = q.OrderByDescending(p => p.Popularity).ThenByDescending(p => p.Id); break;
                case ProductSortType.CustomerRank: q = q.OrderByDescending(p => p.CustomerRating).ThenByDescending(p => p.Id); break;
                case ProductSortType.Price: q = q.OrderByDescending(p => p.Price).ThenByDescending(p => p.Id); break;
            }

            if (!skip.HasValue)
            {
                skip = 0;
            }
            if (!take.HasValue)
            {
                take = 50;
            }


            q = q.Skip(skip.Value);
            q = q.Take(take.Value);

            return Ok(
                q.Select(p => new {
                    p.Id,
                    p.Name,
                    p.Description,
                    Brand = new {
                        p.Brand.Id,
                        p.Brand.Name
                    },
                    Category = new
                    {
                        p.Category.Id,
                        p.Category.Name
                    },

                    p.Price,
                    p.Popularity,
                    p.CustomerRating,
                    Tags = p.Tags.Select(p => p.Tag.Name).ToList(),
                }).ToList()
            );
        }
    }
}
