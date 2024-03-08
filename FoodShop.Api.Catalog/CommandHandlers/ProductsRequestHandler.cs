using FoodShop.Api.Catalog.Commands;
using FoodShop.Api.Catalog.Extensions;
using FoodShop.Api.Catalog.Model;
using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.Catalog.CommandHandlers;

public class ProductsRequestHandler(FoodShopDbContext dbContext) : IRequestHandler<ProductsRequest, IQueryable<Product>>
{
    public async Task<IQueryable<Product>> Handle(ProductsRequest request, CancellationToken cancellationToken)
    {
        var result = dbContext.Products.SetupProductQuery();

        if (!string.IsNullOrEmpty(request.Text))
        {
            result = result.Where(p => EF.Functions.Contains(p.Name, GetFullTextCriteria(request.Text)));
        }
        if (request.Id.HasValue)
        {
            result = result.Where(p => p.Id == request.Id);
        }
        if (request.TagId.HasValue)
        {
            result = result.Where(p => p.Tags.Any(tr => tr.TagId == request.TagId));
        }
        if (request.CategoryId.HasValue)
        {
            result = result.Where(p => p.CategoryId == request.CategoryId.Value);
        }
        if (request.BrandId.HasValue)
        {
            result = result.Where(p => p.BrandId == request.BrandId.Value);
        }

        var sort = request.Sort ?? ProductSortType.Popularity;
        switch (sort)
        {
            case ProductSortType.Popularity: result = result.OrderByDescending(p => p.Popularity).ThenByDescending(p => p.Id); break;
            case ProductSortType.CustomerRank: result = result.OrderByDescending(p => p.CustomerRating).ThenByDescending(p => p.Id); break;
            case ProductSortType.Price: result = result.OrderByDescending(p => p.Price).ThenByDescending(p => p.Id); break;
        }

        result = result
            .Skip(request.Skip ?? 0)
            .Take(request.Take ?? 50);

        return result;
    }

    private static string GetFullTextCriteria(string text)
    {
        text = text.Replace("~", " ");
        return string.Join(" ~ ", text.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(w => $"\"{w}*\""));
    }
}
