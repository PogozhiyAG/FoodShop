using MediatR;

namespace FoodShop.Api.Order.Commands;

public class OrderSetStatusPaidCommand : IRequest
{
    public required string PaymentIntentId { get; set; }
}
