using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StockApp.Application.Configurations;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StockApp.Infra.Data.Identity;

namespace StockApp.Application.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthenticateService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = new JwtSettings();
            configuration.GetSection("Jwt").Bind(_jwtSettings);
        }

        public async Task<TokenResponseDto> Authenticate(UserLoginDto userLoginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(
                userLoginDto.Email, userLoginDto.Password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
                if (user == null) return new TokenResponseDto(false, new List<string> { "Usuário não encontrado." });

                var token = GenerateJwtToken(user);
                return new TokenResponseDto(token, null, _jwtSettings.ExpireMinutes * 60, true);
            }

            return new TokenResponseDto(false, new List<string> { "Credenciais inválidas." });
        }

        public async Task<TokenResponseDto> RegisterUser(UserRegisterDto userRegisterDto)
        {
            var user = new ApplicationUser { UserName = userRegisterDto.Email, Email = userRegisterDto.Email };
            var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                var token = GenerateJwtToken(user);
                return new TokenResponseDto(token, null, _jwtSettings.ExpireMinutes * 60, true);
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return new TokenResponseDto(false, errors);
        }

        public async Task<TokenResponseDto> Logout()
        {
            await _signInManager.SignOutAsync();
            return new TokenResponseDto(true, null);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}