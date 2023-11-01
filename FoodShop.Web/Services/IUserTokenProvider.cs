using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Web.Services;

public interface IUserTokenProvider
{
    IEnumerable<UserToken> GetUserTokens();
}

public class UserTokenProvider : IUserTokenProvider
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly FoodShopDbContext _context;
    private readonly Lazy<IEnumerable<UserToken>> _userTokensLazy;


    public UserTokenProvider(IHttpContextAccessor contextAccessor, FoodShopDbContext context)
    {
        _contextAccessor = contextAccessor;
        _context = context;
        _userTokensLazy = new Lazy<IEnumerable<UserToken>>(LoadUserTokens);
    }

    public IEnumerable<UserToken> GetUserTokens() => _userTokensLazy.Value;

    public IEnumerable<UserToken> LoadUserTokens()
    {
        return _context.UserTokens
            .AsNoTracking()
            .Where(ut => ut.UserId == _contextAccessor.HttpContext!.User.Identity!.Name)
            .ToList();
    }
}