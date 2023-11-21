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
            int? pageSize,
            int? pageNumber,
            ProductSortType? sort
        )
        {
            //var q = tagId.HasValue
            //    ? _db.ProductTagRelations.AsNoTracking()
            //        .Where(r => r.TagId == tagId)
            //        .Include(r => r.Product)
            //            .ThenInclude(r => r.Brand)
            //        .Include(r => r.Product)
            //            .ThenInclude(r => r.Category)
            //        .Include(r => r.Tag)
            //        .Select(r => r.Product)
            //    : _db.Products.AsNoTracking()
            //        .Include(p => p.Tags)
            //        .Include(p => p.Brand)
            //        .Include(p => p.Category);

            var q = _db.Products.AsNoTracking();

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
            //if (tagId.HasValue)
            //{
            //    q = q.Where(p => p.Tags);
            //}
            if (!sort.HasValue)
            {
                sort = ProductSortType.Popularity;
            }
            switch (sort)
            {
                case ProductSortType.Popularity: q = q.OrderByDescending(p => p.Popularity); break;
                case ProductSortType.CustomerRank: q = q.OrderByDescending(p => p.CustomerRating); break;
                case ProductSortType.Price: q = q.OrderByDescending(p => p.Price); break;
            }

            if (!pageSize.HasValue)
            {
                pageSize = 30;
            }
            if (!pageNumber.HasValue)
            {
                pageNumber = 0;
            }

            q = q.Skip(pageSize.Value * pageNumber.Value);
            q = q.Take(pageSize.Value);

            q = q.Include(p => p.Tags).ThenInclude(tr => tr.Tag);
            q = q.Include(p => p.Brand);
            q = q.Include(p => p.Category);

            return Ok(
                q.Select(p => new {
                    p.Id,
                    p.Name,
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
