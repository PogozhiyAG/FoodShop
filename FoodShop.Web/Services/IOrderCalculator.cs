using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using FoodShop.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FoodShop.Web.Services
{
    public interface IOrderCalculator
    {
        public BasketModel Calculate(Basket basket);
    }


    public class OrderCalculator : IOrderCalculator
    {
        private readonly IProductPriceCalculator _productPriceCalculator;
        private readonly FoodShopDbContext _context;

        public OrderCalculator(IProductPriceCalculator productPriceCalculator, FoodShopDbContext context)
        {
            _productPriceCalculator = productPriceCalculator;
            _context = context;
        }




        public BasketModel Calculate(Basket basket)
        {
            BasketModel result = new();


            var productIds = basket.Items.Select(i => i.ProductId).ToList();

            var productsDic = _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Include(p => p.Tags)
                .ToDictionary(p => p.Id);




            result.Items = basket.Items.Select(i => {
                var priceCalculationResult = _productPriceCalculator.Calculate(productsDic[i.ProductId], i.Quantity).CalculationResults.FirstOrDefault();
                return new BasketItemModel()
                {
                    ProductId = i.ProductId,
                    ProductName = productsDic[i.ProductId].Name,
                    Quantity = i.Quantity,
                    Price = productsDic[i.ProductId].Price,
                    CalculatedPrice = priceCalculationResult.Key.Item2,
                    ProductPriceStrategyLink = priceCalculationResult!.Value
                };
            }).ToList();

            var sum = result.Items.Sum(i => i.CalculatedPrice);
            result.TotalAmount = sum;

            return result;
        }

    }

}
