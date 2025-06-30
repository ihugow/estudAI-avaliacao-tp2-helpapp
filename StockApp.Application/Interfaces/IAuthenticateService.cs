using StockApp.Application.DTOs;

namespace StockApp.Application.Interfaces
{
    public interface IAuthenticateService
    {
        Task<TokenResponseDto> Authenticate(UserLoginDto userLoginDto);
        Task<TokenResponseDto> RegisterUser(UserRegisterDto userRegisterDto);
        Task<TokenResponseDto> Logout();
    }
}