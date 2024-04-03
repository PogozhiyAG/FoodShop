using FoodShop.Api.Catalog.Model.Internal;
using FoodShop.Api.Catalog.Commands;
using MediatR;
using FoodShop.Api.Catalog.Services;
using FoodShop.Core.Models;

namespace FoodShop.Api.Catalog.CommandHandlers;

public class ProductsCalculationRequestHandler : IRequestHandler<ProductsCalculationRequest, IQueryable<ProductCalculationItem>>
{
    private readonly IProductPriceStrategyProvider _priceStrategyProvider;
    private readonly IUserTokenTypesProvider _userTokenTypesProvider;

    public ProductsCalculationRequestHandler(IProductPriceStrategyProvider priceStrategyProvider, IUserTokenTypesProvider userTokenTypesProvider)
    {
        _priceStrategyProvider = priceStrategyProvider;
        _userTokenTypesProvider = userTokenTypesProvider;
    }

    public async Task<IQueryable<ProductCalculationItem>> Handle(ProductsCalculationRequest request, CancellationToken cancellationToken)
    {
        var tokenTypes = await _userTokenTypesProvider.GetUserTokenTypes();

        var result = request.Products
            .Select(p => new
            {
                Product = p,
                PriceStrategyLink = _priceStrategyProvider.GetStrategyLink(p, tokenTypes), //TODO make async
                Quantity = request.GetProductQuantity != null ? request.GetProductQuantity(p) : 1
            })
            .Select(d => new ProductCalculationItem()
            {
                Product = d.Product,
                PriceStrategyLink = d.PriceStrategyLink,
                Quantity = d.Quantity,
                Amount = ProductPriceStrategy.Default.GetAmount(d.Product.Price, d.Quantity),
                OfferPrice = d.PriceStrategyLink.ProductPriceStrategy.GetAmount(d.Product.Price, 1),
                OfferAmount = d.PriceStrategyLink.ProductPriceStrategy.GetAmount(d.Product.Price, d.Quantity),
            });

        return result;
    }
}
