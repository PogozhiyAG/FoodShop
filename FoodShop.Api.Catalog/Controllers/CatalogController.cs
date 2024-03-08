using FoodShop.Api.Catalog.Model;
using FoodShop.Infrastructure.Data;
using FoodShop.Api.Catalog.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodShop.Api.Catalog.Extensions;
using MediatR;
using FoodShop.Api.Catalog.Commands;

namespace FoodShop.Api.Catalog.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class CatalogController (
    IDbContextFactory<FoodShopDbContext> _dbContextFactory,
    IMediator _mediator
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        int? Id,
        int? categoryId,
        int? brandId,
        int? tagId,
        string? text,
        int? skip,
        int? take,
        ProductSortType? sort
    )
    {
        var products = await _mediator.Send(new ProductsRequest()
        {
            Id = Id,
            CategoryId = categoryId,
            BrandId = brandId,
            TagId = tagId,
            Text = text,
            Skip = skip,
            Take = take,
            Sort = sort
        });

        var productCalculationItems = await _mediator.Send(new Commands.ProductsCalculationRequest()
        {
            Products = products
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
}
