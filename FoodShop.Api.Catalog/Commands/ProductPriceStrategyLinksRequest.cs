using FoodShop.Api.Catalog.Services;
using FoodShop.Core.Models;
using MediatR;

namespace FoodShop.Api.Catalog.Commands;

public class ProductPriceStrategyLinksRequest : IRequest<ProductPriceStrategyLinksResponse>
{
}

public class ProductPriceStrategyLinksResponse
{
    public required Dictionary<StrategyKey, ProductPriceStrategyLink> StrategyLinks { get; init; }
}