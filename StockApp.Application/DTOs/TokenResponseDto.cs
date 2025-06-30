using System.Collections.Generic;

namespace StockApp.Application.DTOs
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }

        public TokenResponseDto()
        {
            Errors = new List<string>();
        }

        public TokenResponseDto(string token, string refreshToken, int expiresIn, bool success)
        {
            Token = token;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
            Success = success;
            Errors = new List<string>();
        }

        public TokenResponseDto(bool success, List<string> errors)
        {
            Success = success;
            Errors = errors ?? new List<string>();
        }
    }
}