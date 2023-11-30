using FoodShop.Api.Auth.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FoodShop.Api.Auth.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<JwtBearerOptions> _jwtBearerOptions;
    private readonly IConfiguration _configuration;

    public AuthenticationController(UserManager<ApplicationUser> userManager, IOptions<JwtBearerOptions> jwtBearerOptions, IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtBearerOptions = jwtBearerOptions;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegistrationRequest registrationRequest)
    {
        var existingUser = await _userManager.FindByNameAsync(registrationRequest.UserName);
        if (existingUser != null)
        {
            return Conflict("User already exists");
        }

        var user = new ApplicationUser()
        {
            UserName = registrationRequest.UserName
        };

        var result = await _userManager.CreateAsync(user, registrationRequest.Password);

        if (result.Succeeded)
        {
            return Ok("User successfully created");
        }

        return StatusCode(StatusCodes.Status500InternalServerError, result);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var user = await _userManager.FindByNameAsync(loginRequest.UserName);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            return Unauthorized();
        }

        var jwtToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpired = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration["JWT:RefreshTokenLifetime"]));

        await _userManager.UpdateAsync(user);

        var result = new LoginResponse()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = refreshToken
        };

        return Ok(result);
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody]RefreshRequest refreshRequest)
    {
        var principal = GetPrincipalFromToken(refreshRequest.Token);
        var userName = principal.Identity?.Name;

        if(userName == null)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByNameAsync(userName);

        if(user == null || user.RefreshToken != refreshRequest.RefreshToken || user.RefreshTokenExpired < DateTime.UtcNow)
        {
            return Unauthorized();
        }

        var jwtToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        await _userManager.UpdateAsync(user);

        var result = new RefreshResponse()
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
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var result = new JwtSecurityToken(
            issuer:  _jwtBearerOptions.Value.TokenValidationParameters.ValidIssuer,
            audience: _jwtBearerOptions.Value.TokenValidationParameters.ValidAudience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.Parse(_configuration["JWT:TokenLifetime"])),
            signingCredentials: new SigningCredentials(_jwtBearerOptions.Value.TokenValidationParameters.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
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


    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var validationParameters = _jwtBearerOptions.Value.TokenValidationParameters.Clone();
        validationParameters.ValidateLifetime = false;

        return new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);
    }
}
