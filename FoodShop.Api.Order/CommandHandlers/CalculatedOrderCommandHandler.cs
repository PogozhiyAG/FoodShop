using FoodShop.Api.Order.Commands;
using FoodShop.Api.Order.Services.Calculation;
using MediatR;

namespace FoodShop.Api.Order.CommandHandlers;

public class CalculatedOrderCommandHandler : IRequestHandler<CalculateOrderCommand, OrderCalculationContext>
{
    private readonly IOrderCalculator _orderCalculator;

    public CalculatedOrderCommandHandler(IOrderCalculator orderCalculator)
    {
        _orderCalculator = orderCalculator;
    }

    public async Task<OrderCalculationContext> Handle(CalculateOrderCommand request, CancellationToken cancellationToken)
    {
        var result = await _orderCalculator.CalculateOrder(request.Order);
        return result;
    }
}
