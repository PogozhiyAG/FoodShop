using FoodShop.Api.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks.Sources;

namespace FoodShop.Api.Auth.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthenticationController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegistrationModel model)
    {
        var existingUser = await _userManager.FindByNameAsync(model.UserName);
        if (existingUser != null)
        {
            return Conflict("User already exists");
        }

        var user = new ApplicationUser()
        {
            UserName = model.UserName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            Ok("User successfully created");
        }

        return StatusCode(StatusCodes.Status500InternalServerError, result);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized();
        }

        var jwtToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpired = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);

        var result = new LoginResponse()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = refreshToken
        };
        return Ok(result);
    }

    private JwtSecurityToken GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(";oikehjnoiwnrg'ponwwjhuIOHUIOUIYU&%^&$^FUJhkj;lksrmgt;lerjtle;rjtlerjtle;r;mev;lem;lrjhel;rj;o;pj'p-f3-"));

        var result = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(60),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return result;
    }

    private string GenerateRefreshToken()
    {
        var array = new byte[64];
        var generator = RandomNumberGenerator.Create();
        generator.GetBytes(array);
        return Convert.ToBase64String(array);
    }
}
