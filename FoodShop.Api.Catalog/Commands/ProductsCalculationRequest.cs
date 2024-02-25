using FoodShop.Api.Catalog.Model.Internal;
using FoodShop.Core.Models;
using MediatR;

namespace FoodShop.Api.Catalog.Commands;

public class ProductsCalculationRequest : IRequest<IQueryable<ProductCalculationItem>>
{
    public required IQueryable<Product> Products { get; set; }
    public Func<Product, int>? GetProductQuantity { get; set; }
}
