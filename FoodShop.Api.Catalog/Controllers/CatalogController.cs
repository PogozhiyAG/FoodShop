using FoodShop.Api.Catalog.Model;
using FoodShop.Api.Catalog.Services;
using FoodShop.Infrastructure.Data;
using FoodShop.Api.Catalog.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FoodShop.Core.Models;

namespace FoodShop.Api.Catalog.Controllers;

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
        var tokenTypes = await GetTokenTypes();

        using var db = await _dbContextFactory.CreateDbContextAsync();

        var q = db.Products.SetupProductQuery();

        if (!string.IsNullOrEmpty(text))
        {
            q = q.Where(p => EF.Functions.Contains(p.Name, GetFullTextCriteria(text)));
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

        var result = q
            .Skip(skip.Value)
            .Take(take.Value)
            .Select(p => p.MapToOfferedProductDto(
                _priceStrategyProvider.GetStrategyLink(p, tokenTypes)
             ))
            .ToList();

        return Ok(result);
    }



    [HttpGet("calculate")]
    public async Task<IActionResult> Calculate([FromBody] CalculateRequest request)
    {
        var tokenTypes = await GetTokenTypes();

        using var db = await _dbContextFactory.CreateDbContextAsync();

        var productIds = request.Items.Keys.Select(k => int.Parse(k)).ToList();

        var q = db.Products
            .SetupProductQuery()
            .Where(p => productIds.Contains(p.Id))
            .Select(p => p.MapToOfferedProductBatchDto(
                _priceStrategyProvider.GetStrategyLink(p, tokenTypes),
                request.Items[p.Id.ToString()]
             ))
            .ToList();

        return Ok(q);
    }



    private async Task<IEnumerable<string>> GetTokenTypes()
    {
        var principal = _httpContextAccessor.HttpContext!.User;
        var isAnonymous = principal.Claims.Any(c => c.Type == ClaimTypes.Anonymous);
        if (isAnonymous)
        {
            return Array.Empty<string>();
        }

        return await _customerProfile.GetTokenTypes(principal.Identity.Name);
    }

    private string GetFullTextCriteria(string text)
    {
        text = text.Replace("~", " ");
        return string.Join(" ~ ", text.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(w => $"\"{w}*\""));
    }
}


public static class ProductQueryExtensions
{
    public static IQueryable<Product> SetupProductQuery(this IQueryable<Product> products) => products
        .AsNoTracking()
        .Include(p => p.Tags).ThenInclude(t => t.Tag)
        .Include(p => p.Brand)
        .Include(p => p.Category);
}
