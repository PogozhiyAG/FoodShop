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
        var userId = GetUserId();
        var result = await _db.CustomerTokens.AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("valid-token-types")]
    public async Task<IActionResult> GetValidTokenTypes()
    {
        var userId = GetUserId();
        var moment = DateTime.Now;

        var result = await _db.CustomerTokens.AsNoTracking()
            .Where(t => t.UserId == userId)
            .Where(t => t.ValidFrom <= moment && (t.ValidTo == null || t.ValidTo > moment))
            .Select(t => t.TokenType.Code)
            .Distinct()
            .ToListAsync();
        return Ok(result);
    }

    private string GetUserId()
    {
        return Request.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}
