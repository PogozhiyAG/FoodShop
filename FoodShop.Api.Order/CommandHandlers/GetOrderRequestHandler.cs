using FoodShop.Api.Order.Commands;
using FoodShop.Api.Order.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.Order.CommandHandlers;

public class GetOrderRequestHandler : IRequestHandler<GetOrderCommand, Model.Order>
{
    //TODO OrderDbContext instead?
    private readonly IDbContextFactory<OrderDbContext> _dbContextFactory;

    public GetOrderRequestHandler(IDbContextFactory<OrderDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Model.Order> Handle(GetOrderCommand request, CancellationToken cancellationToken)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var result = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.DeliveryInfo)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId);

        return result;

    }
}
