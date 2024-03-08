using FoodShop.Api.Catalog.Model;
using FoodShop.Core.Models;
using MediatR;

namespace FoodShop.Api.Catalog.Commands;

public class ProductsRequest : IRequest<IQueryable<Product>>
{
    public int? Id { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? TagId { get; set; }
    public string? Text { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
    public ProductSortType? Sort { get; set; }
}
