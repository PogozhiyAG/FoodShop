using FoodShop.Api.Catalog.Model.Internal;
using FoodShop.Api.Catalog.Commands;
using MediatR;
using FoodShop.Api.Catalog.Services;
using FoodShop.Core.Models;

namespace FoodShop.Api.Catalog.CommandHandlers;

public class ProductsCalculationRequestHandler(
    IMediator _mediator,
    IUserTokenTypesProvider _userTokenTypesProvider,
    IProductPriceStrategyProvider _priceStrategyProvider
    ) : IRequestHandler<ProductsCalculationRequest, IQueryable<ProductCalculationItem>>
{
    public async Task<IQueryable<ProductCalculationItem>> Handle(ProductsCalculationRequest request, CancellationToken cancellationToken)
    {
        var tokenTypes = _userTokenTypesProvider.GetUserTokenTypes();
        var strategiesLinksDictionary = _mediator.Send(new ProductPriceStrategyLinksRequest());

        await Task.WhenAll(tokenTypes, strategiesLinksDictionary);

        var result = request.Products
            .Select(p => new
            {
                Product = p,
                PriceStrategyLink = _priceStrategyProvider.GetStrategyLink(p, tokenTypes.Result, strategiesLinksDictionary.Result.StrategyLinks),
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
