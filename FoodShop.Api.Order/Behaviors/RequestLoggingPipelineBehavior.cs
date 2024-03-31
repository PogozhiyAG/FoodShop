using MediatR;

namespace FoodShop.Api.Order.Behaviors;

public class RequestLoggingPipelineBehavior<TRequest, TResponse> (
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger
    )
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request {RequestName}", request);
        var response =  await next();
        _logger.LogInformation("Response {RequestName}", response);

        return response;
    }
}
