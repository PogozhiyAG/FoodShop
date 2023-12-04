﻿using FoodShop.Api.Catalog.Services;
using FoodShop.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.Catalog.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IDbContextFactory<FoodShopDbContext> _dbContextFactory;
        private readonly IProductPriceStrategyProvider _priceStrategyProvider;
        private readonly ICustomerProfile _customerProfile;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CatalogController(
            IDbContextFactory<FoodShopDbContext> dbContextFactory,
            IProductPriceStrategyProvider priceStrategyProvider,
            ICustomerProfile customerProfile,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContextFactory = dbContextFactory;
            _priceStrategyProvider = priceStrategyProvider;
            _customerProfile = customerProfile;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            int? categoryId,
            int? brandId,
            int? tagId,
            string? text,
            int? skip,
            int? take,
            ProductSortType? sort
        )
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();

            var q = db.Products.AsNoTracking();

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

            var tokenTypes = await _customerProfile.GetTokenTypes(_httpContextAccessor.HttpContext.User.Identity.Name);

            q = q
                .Include(p => p.Tags)
                .Include(p => p.Brand)
                .Include(p => p.Category);

            return Ok(
                q.Select(p => new
                {
                    Product = p,
                    OfferLink = _priceStrategyProvider.GetStrategyLink(p, tokenTypes)
                })
                .ToList()
                .Select(p =>
                    new
                    {
                        p.Product.Id,
                        p.Product.Name,
                        p.Product.Description,
                        Brand = new
                        {
                            p.Product.Brand?.Id,
                            p.Product.Brand?.Name
                        },
                        Category = new
                        {
                            p.Product.Category?.Id,
                            p.Product.Category?.Name
                        },
                        p.Product.Price,
                        p.Product.Popularity,
                        p.Product.CustomerRating,
                        //TODO
                        OfferCode = p.OfferLink.TokenTypeCode,
                        OfferPrice = p.OfferLink.ProductPriceStrategy.GetAmount(p.Product.Price, 1)
                    }
                )
            ) ;
        }
    }
}
