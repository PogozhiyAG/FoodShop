using FoodShop.Core.Models;
using System.Text.Json.Serialization;

namespace FoodShop.Web.Services;

public interface IProductPriceCalculator
{
    ProductPriceCalculatorResult Calculate(Product product, int quantity);
}


public class ProductPriceCalculatorResult
{
    public decimal? ResultPrice { get => CalculationResults.FirstOrDefault()!.Key.Item2; }
    [JsonIgnore]
    public SortedList<(int, decimal), ProductPriceStrategyLink> CalculationResults { get; set; } = new();
}


public class ProductPriceCalculator : IProductPriceCalculator
{
    private readonly IUserTokenProvider _userTokenProvider;
    private readonly IProductPriceStrategyProvider _productPriceStrategyProvider;

    public ProductPriceCalculator(IUserTokenProvider userTokenProvider, IProductPriceStrategyProvider productPriceStrategyProvider)
    {
        _userTokenProvider = userTokenProvider;
        _productPriceStrategyProvider = productPriceStrategyProvider;
    }

    public ProductPriceCalculatorResult Calculate(Product product, int quantity = 1)
    {
        var result = new ProductPriceCalculatorResult();

        var tokenTypeIds = _userTokenProvider.GetUserTokens()
            .Select(x => x.TokenTypeId)
            .Concat(new[] { 0 });

        foreach (var strategyLink in _productPriceStrategyProvider.GetStrategyLinks(product, tokenTypeIds))
        {
            var amount = strategyLink.ProductPriceStrategy.GetAmount(product.Price, quantity);
            var key = (strategyLink.Priority, amount);
            if (!result.CalculationResults.ContainsKey(key))
            {
                result.CalculationResults.Add(key, strategyLink);
            }
        }

        return result;
    }
}
