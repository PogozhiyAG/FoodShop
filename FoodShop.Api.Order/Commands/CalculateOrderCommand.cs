using FoodShop.Api.Order.Services.Calculation;
using MediatR;

namespace FoodShop.Api.Order.Commands;

public class CalculateOrderCommand : IRequest<OrderCalculationContext>
{
    public required Model.Order Order { get; set; }
}