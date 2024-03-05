using FoodShop.Api.Warehouse.Commands;
using FoodShop.Order.Grpc;
using MediatR;

namespace FoodShop.Api.Warehouse.CommandHandlers;

public class CreateParcelRequestHandler(
    OrderService.OrderServiceClient _orderServiceClient
) : IRequestHandler<CreateParcelRequest, CreateParcelResponse>
{

    public async Task<CreateParcelResponse> Handle(CreateParcelRequest request, CancellationToken cancellationToken)
    {
        var grpcRequest = new OrderRequest() {
            OrderId = request.OrderId.ToString()
        };

        var order = await _orderServiceClient.GetOrderAsync(grpcRequest);

        Console.WriteLine($"Emulate hard work with order {order.Id}");

        var result = new CreateParcelResponse() {
            ParcelId = Guid.NewGuid()
        };

        return result;
    }
}
