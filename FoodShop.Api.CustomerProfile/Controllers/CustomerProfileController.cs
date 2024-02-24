using FoodShop.Api.CustomerProfile.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.CustomerProfile.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CustomerProfileController : ControllerBase
{
    private readonly CustomerProfileDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomerProfileController(CustomerProfileDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetUserName() => _httpContextAccessor.HttpContext!.User.Identity!.Name!;

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userName = GetUserName();
        var moment = DateTime.Now;

        var delivery = await _db.CustomerDeliveryInfos.AsNoTracking()
            .Where(p => p.UserName == userName)
            .FirstOrDefaultAsync();
        //TODO: Refactoring
        var tokenTypes = await _db.CustomerTokens.AsNoTracking()
            .Where(x => x.UserName == userName)
            .Where(t => t.ValidFrom <= moment && (t.ValidTo == null || t.ValidTo > moment))
            .Select(t => t.TokenType.Code)
            .Distinct()
            .ToListAsync();

        var result = new
        {
            delivery,
            tokenTypes
        };

        return Ok(result);
    }

    //TODO: user from auth
    [HttpGet("valid-token-types/{userName}")]
    public async Task<IActionResult> GetValidTokenTypes(string userName)
    {
        var moment = DateTime.Now;
        //TODO: Refactoring
        var result = await _db.CustomerTokens.AsNoTracking()
            .Where(x => x.UserName == userName)
            .Where(t => t.ValidFrom <= moment && (t.ValidTo == null || t.ValidTo > moment))
            .Select(t => t.TokenType.Code)
            .Distinct()
            .ToListAsync();

        return Ok(result);
    }
}
