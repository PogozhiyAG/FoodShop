using FoodShop.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.Catalog.Extensions;

public static class ProductQueryExtensions
{
    public static IQueryable<Product> SetupProductQuery(this IQueryable<Product> products) => products
        .AsNoTracking()
        .Include(p => p.Tags).ThenInclude(t => t.Tag)
        .Include(p => p.Brand)
        .Include(p => p.Category);
}