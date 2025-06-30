using Microsoft.AspNetCore.Identity;
using System;

namespace StockApp.Infra.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
