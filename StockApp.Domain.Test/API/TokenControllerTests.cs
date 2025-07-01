using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Controllers;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StockApp.Domain.Test.API
{
    public class TokenControllerTests
    {
        private readonly Mock<IAuthenticateService> _mockAuthenticateService;
        private readonly TokenController _tokenController;

        public TokenControllerTests()
        {
            _mockAuthenticateService = new Mock<IAuthenticateService>();
            _tokenController = new TokenController(_mockAuthenticateService.Object);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDto { Email = "test@example.com", Password = "Password123!", ConfirmPassword = "Password123!" };
            var tokenResponse = new TokenResponseDto("token", "refreshToken", 3600, true);

            _mockAuthenticateService.Setup(x => x.RegisterUser(userRegisterDto))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _tokenController.Register(userRegisterDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<TokenResponseDto>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Equal(tokenResponse.Token, returnValue.Token);
        }

        [Fact]
        public async Task Register_InvalidUser_ReturnsBadRequestResult()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDto { Email = "invalid@example.com", Password = "short", ConfirmPassword = "short" };
            var tokenResponse = new TokenResponseDto(false, new List<string> { "Password too short." });

            _mockAuthenticateService.Setup(x => x.RegisterUser(userRegisterDto))
                .ReturnsAsync(tokenResponse);

            _tokenController.ModelState.AddModelError("Password", "Password too short.");

            // Act
            var result = await _tokenController.Register(userRegisterDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<TokenResponseDto>(badRequestResult.Value);
            Assert.False(returnValue.Success);
            Assert.Contains("Password too short.", returnValue.Errors);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var userLoginDto = new UserLoginDto { Email = "test@example.com", Password = "Password123!" };
            var tokenResponse = new TokenResponseDto("token", "refreshToken", 3600, true);

            _mockAuthenticateService.Setup(x => x.Authenticate(userLoginDto))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _tokenController.Login(userLoginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<TokenResponseDto>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.Equal(tokenResponse.Token, returnValue.Token);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorizedResult()
        {
            // Arrange
            var userLoginDto = new UserLoginDto { Email = "test@example.com", Password = "WrongPassword" };
            var tokenResponse = new TokenResponseDto(false, new List<string> { "Invalid credentials." });

            _mockAuthenticateService.Setup(x => x.Authenticate(userLoginDto))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _tokenController.Login(userLoginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var returnValue = Assert.IsType<TokenResponseDto>(unauthorizedResult.Value);
            Assert.False(returnValue.Success);
            Assert.Contains("Invalid credentials.", returnValue.Errors);
        }

        // TODO: Add tests for RefreshToken and Logout
    }
}