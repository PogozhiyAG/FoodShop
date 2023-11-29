﻿using Microsoft.AspNetCore.Identity;

namespace FoodShop.Api.Auth.Models;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpired { get; set; }
}
