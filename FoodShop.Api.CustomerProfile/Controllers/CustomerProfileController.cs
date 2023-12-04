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

    public CustomerProfileController(CustomerProfileDbContext db)
    {
        _db = db;
    }

    [HttpGet("valid-token-types/{userName}")]
    public async Task<IActionResult> GetValidTokenTypes(string userName)
    {
        var moment = DateTime.Now;

        var result = await _db.CustomerTokens.AsNoTracking()
            .Where(x => x.UserName == userName)
            .Where(t => t.ValidFrom <= moment && (t.ValidTo == null || t.ValidTo > moment))
            .Select(t => t.TokenType.Code)
            .Distinct()
            .ToListAsync();

        return Ok(result);
    }
}
