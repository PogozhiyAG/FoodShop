using FoodShop.Api.Catalog.Commands;
using FoodShop.Api.Catalog.Services;
using FoodShop.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.Catalog.CommandHandlers;

public class ProductPriceStrategyLinksRequestHandler(IDbContextFactory<FoodShopDbContext> _dbContextFactory) :
    IRequestHandler<ProductPriceStrategyLinksRequest, ProductPriceStrategyLinksResponse>
{
    public async Task<ProductPriceStrategyLinksResponse> Handle(ProductPriceStrategyLinksRequest request, CancellationToken cancellationToken)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var result = db.ProductPriceStrategyLinks
            .AsNoTracking()
            .Include(l => l.ProductPriceStrategy)
            .ToDictionary(l => l.MapToStrategyKey());

        return new ProductPriceStrategyLinksResponse() {
            StrategyLinks = result
        };
    }
}
