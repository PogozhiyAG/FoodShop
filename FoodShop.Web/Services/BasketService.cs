using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;

namespace FoodShop.Web.Services
{
    public class BasketService : IBasketService
    {
        private readonly FoodShopDbContext _context;

        public BasketService(FoodShopDbContext context)
        {
            _context = context;
        }

        public void AddProduct(string basketId, Guid productId, decimal price)
        {
            throw new NotImplementedException();
        }


        public void RemoveProduct(string basketId, Guid productId)
        {
            throw new NotImplementedException();
        }

        public void SetProduct(string basketId, Guid productId, decimal price)
        {
            throw new NotImplementedException();
        }

        public Basket GetBasketById(string basketId)
        {
            throw new NotImplementedException();
        }
    }
}
