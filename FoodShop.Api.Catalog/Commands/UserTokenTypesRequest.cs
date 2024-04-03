using MediatR;

namespace FoodShop.Api.Catalog.Commands;

public class UserTokenTypesRequest : IRequest<UserTokenTypesResponse>
{
    public string? UserName { get; set; }
}

public class UserTokenTypesResponse
{
    public IEnumerable<string>? TokenTypes { get; set; }
}

