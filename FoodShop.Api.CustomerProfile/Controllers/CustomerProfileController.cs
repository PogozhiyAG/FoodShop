using FoodShop.Api.CustomerProfile.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodShop.Api.CustomerProfile.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CustomerProfileController : ControllerBase
{
    private readonly CustomerProfileDbContext _db;

    public CustomerProfileController(CustomerProfileDbContext db)
    {
        _db = db;
    }


    [HttpGet("tokens")]
    public async Task<IActionResult> GetTokens()
    {
        var userName = GetUserName();
        var result = await _db.CustomerTokens.AsNoTracking()
            .Where(x => x.UserName == userName)
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("valid-token-types")]
    public async Task<IActionResult> GetValidTokenTypes()
    {
        var userName = GetUserName();
        var moment = DateTime.Now;

        var result = await _db.CustomerTokens.AsNoTracking()
            .Where(x => x.UserName == userName)
            .Where(t => t.ValidFrom <= moment && (t.ValidTo == null || t.ValidTo > moment))
            .Select(t => t.TokenType.Code)
            .Distinct()
            .ToListAsync();
        return Ok(result);
    }

    private string GetUserName() => Request.HttpContext.User.Identity.Name;
}
