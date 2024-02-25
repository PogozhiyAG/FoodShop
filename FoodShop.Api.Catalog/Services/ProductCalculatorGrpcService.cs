using FoodShop.Api.Catalog.Extensions;
using FoodShop.Api.Catalog.Mapping;
using FoodShop.Catalog.Grpc;
using FoodShop.Infrastructure.Data;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.Catalog.Services;


[Authorize]
public class ProductCalculatorGrpcService : ProductCalculator.ProductCalculatorBase
{
    private readonly IDbContextFactory<FoodShopDbContext> _dbContextFactory;
    private readonly IMediator _mediator;

    public ProductCalculatorGrpcService(IDbContextFactory<FoodShopDbContext> dbContextFactory, IMediator mediator)
    {
        _dbContextFactory = dbContextFactory;
        _mediator = mediator;
    }

    public override async Task<ProductCalculationResponse> Calculate(ProductCalculationRequest request, ServerCallContext context)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync();

        //TODO check without ToList
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var productsDic = request.Items.ToDictionary(i => i.ProductId, i => i.Quantity);

        var products = db.Products
           .SetupProductQuery()
           .Where(p => productIds.Contains(p.Id));

        var productCalculationItems = await _mediator.Send(new Commands.ProductsCalculationRequest() {
            Products = products,
            GetProductQuantity = p => productsDic[p.Id]
        });

        var items = productCalculationItems
            .Select(p => p.ToProductCalculationResponseItem())
            .ToList();

        var result = new ProductCalculationResponse();
        result.Items.AddRange(items);
        return result;
    }
}
