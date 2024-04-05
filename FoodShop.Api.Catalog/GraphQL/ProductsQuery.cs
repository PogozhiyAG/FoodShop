using FoodShop.Api.Catalog.Commands;
using FoodShop.Api.Catalog.Dto;
using FoodShop.Api.Catalog.Mapping;
using FoodShop.Api.Catalog.Model;
using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using HotChocolate.Authorization;
using MediatR;

namespace FoodShop.Api.Catalog.GraphQL;

[Authorize]
public class ProductsQuery
{
    public async Task<IEnumerable<Product>> GetProducts(
        [Service] IMediator _mediator,
        int? Id,
        int? categoryId,
        int? brandId,
        int? tagId,
        string? text,
        int? skip,
        int? take,
        ProductSortType? sort
    )
    {
        var products = await _mediator.Send(new ProductsRequest()
        {
            Id = Id,
            CategoryId = categoryId,
            BrandId = brandId,
            TagId = tagId,
            Text = text,
            Skip = skip,
            Take = take,
            Sort = sort
        });

        return products;
    }

    //TODO: finish this
    //[UsePaging]
//    [UseProjection]
//    [UseFiltering]
//    [UseSorting]
//    public async Task<IQueryable<Product>> GetProd(
//        [Service] FoodShopDbContext _db
//    )
//    {
//        return _db.Products.Take(100);
//    }
}