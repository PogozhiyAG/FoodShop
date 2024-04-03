using FoodShop.Api.Catalog.Services;
using FoodShop.Core.Models;
using MediatR;

namespace FoodShop.Api.Catalog.Commands;

public class ProductPriceStrategyLinksRequest : IRequest<ProductPriceStrategyLinksResponse>
{
}

public class ProductPriceStrategyLinksResponse
{
    public Dictionary<StrategyKey, ProductPriceStrategyLink> Result { get; set; }
}