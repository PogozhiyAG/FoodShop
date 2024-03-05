using MediatR;

namespace FoodShop.Api.Warehouse.Commands;

public record CreateParcelRequest : IRequest<CreateParcelResponse>
{
    public Guid OrderId { get; set; }
}


public record CreateParcelResponse
{
    public Guid ParcelId { get; set; }
}