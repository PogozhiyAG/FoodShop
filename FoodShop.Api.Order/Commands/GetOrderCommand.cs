using MediatR;

namespace FoodShop.Api.Order.Commands;

public class GetOrderCommand : IRequest<Model.Order>
{
    public Guid OrderId { get; set; }
}
