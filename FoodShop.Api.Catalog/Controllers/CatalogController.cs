using FoodShop.Api.Catalog.Model;
using FoodShop.Api.Catalog.Services;
using FoodShop.Infrastructure.Data;
using FoodShop.Api.Catalog.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FoodShop.Core.Models;
using FoodShop.Api.Catalog.Extensions;
using MediatR;
using Azure.Core;

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
    private readonly IMediator _mediator;

    public CatalogController(
        IDbContextFactory<FoodShopDbContext> dbContextFactory,
        IProductPriceStrategyProvider priceStrategyProvider,
        ICustomerProfile customerProfile,
        IHttpContextAccessor httpContextAccessor,
        IMediator mediator)
    {
        _dbContextFactory = dbContextFactory;
        _priceStrategyProvider = priceStrategyProvider;
        _customerProfile = customerProfile;
        _httpContextAccessor = httpContextAccessor;
        _mediator = mediator;
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

        q = q
            .Skip(skip.Value)
            .Take(take.Value);

        var productCalculationItems = await _mediator.Send(new Commands.ProductsCalculationRequest()
        {
            Products = q
        });

        var result = productCalculationItems
            .Select(p => p.MapToOfferedProductDto())
            .ToList();

        return Ok(result);
    }



    [HttpGet("calculate")]
    public async Task<IActionResult> Calculate([FromBody] CalculateRequest request)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();

        //TODO check without ToList
        var productIds = request.Items.Keys.Select(k => int.Parse(k)).ToList();

        var products = db.Products
            .SetupProductQuery()
            .Where(p => productIds.Contains(p.Id));

        var productCalculationItems = await _mediator.Send(new Commands.ProductsCalculationRequest() {
            Products = products,
            GetProductQuantity = p => request.Items[p.Id.ToString()]
        });

        var items = productCalculationItems
            .Select(p => p.MapToOfferedProductBatchDto())
            .ToList();

        return Ok(items);
    }


    private string GetFullTextCriteria(string text)
    {
        text = text.Replace("~", " ");
        return string.Join(" ~ ", text.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(w => $"\"{w}*\""));
    }
}
