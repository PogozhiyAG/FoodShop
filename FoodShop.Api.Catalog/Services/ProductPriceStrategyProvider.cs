using FoodShop.Api.Catalog.Commands;
using FoodShop.Core.Models;
using MediatR;

namespace FoodShop.Api.Catalog.Services;

/// <summary>
/// Finds price strategy for a product for giving token types
/// </summary>
public interface IProductPriceStrategyProvider
{
    ProductPriceStrategyLink GetStrategyLink(Product product, IEnumerable<string> tokenTypeIds, Dictionary<StrategyKey, ProductPriceStrategyLink> strategiesLinksDictionary);
}

public class ProductPriceStrategyProvider() : IProductPriceStrategyProvider
{
    public ProductPriceStrategyLink GetStrategyLink(Product product, IEnumerable<string> tokenTypeIds, Dictionary<StrategyKey, ProductPriceStrategyLink> strategiesLinksDictionary)
    {
        ProductPriceStrategyLink result = ProductPriceStrategyLink.Default;

        foreach (var tokenTypeId in tokenTypeIds.Append(null))
        {
            foreach (var referenceId in product.ExtractEntityReferences().Append(null))
            {
                var key = new StrategyKey(tokenTypeId, referenceId);
                if (strategiesLinksDictionary.TryGetValue(key, out var value))
                {
                    if (value.Priority > result.Priority)
                    {
                        result = value;
                    }
                }
            }
        }

        return result;
    }
}


internal static class ProductExtensions
{
    public static IEnumerable<EntityReference?> ExtractEntityReferences(this Product product)
    {
        yield return new EntityReference(EntityTypeCode.Product, product.Id);

        if (product.Tags != null)
        {
            foreach (var tag in product.Tags)
            {
                yield return new EntityReference(EntityTypeCode.Tag, tag.TagId);
            }
        }

        if (product.CategoryId.HasValue)
        {
            yield return new EntityReference(EntityTypeCode.ProductCategory, product.CategoryId.Value);
        }

        if (product.BrandId.HasValue)
        {
            yield return new EntityReference(EntityTypeCode.Brand, product.BrandId.Value);
        }
    }
}

public record struct EntityReference(
   EntityTypeCode? ReferenceType,
   int? ReferenceId
);

public record struct StrategyKey(
    string? TokenTypeCode,
    EntityReference? EntityReference
);

public static class ProductPriceStrategyLinkExtensions
{
    public static StrategyKey MapToStrategyKey(this ProductPriceStrategyLink productPriceStrategyLink) =>
        new StrategyKey(
            productPriceStrategyLink.TokenTypeCode,
            productPriceStrategyLink.ReferenceType != null
                ? new EntityReference(productPriceStrategyLink.ReferenceType, productPriceStrategyLink.ReferenceId)
                : null
        );
}